﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Diagnostics;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Persistence;
using FlexBus.Serialization;
using FlexBus.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlexBus.Consumer.Internal;

internal class ConsumerRegister : IConsumerRegister
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly IConsumerClientFactory _consumerClientFactory;
    private readonly ISerializer _serializer;
    private readonly IDataStorage _storage;
    private readonly IConsumerServiceSelector _selector;
    private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);
    private readonly FlexBusOptions _options;
    private readonly ConsumerOptions _consumerOptions;
    private readonly ISubscribeDispatcher _dispatcher;

    private CancellationTokenSource _cts;
    private BrokerAddress _serverAddress;
    private Task _compositeTask;
    private bool _disposed;
    private static bool _isHealthy = true;

    // diagnostics listener
    // ReSharper disable once InconsistentNaming
    private static readonly DiagnosticListener s_diagnosticListener = new(CapDiagnosticListenerNames.DiagnosticListenerName);

    public ConsumerRegister(
        ILogger<ConsumerRegister> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _options = serviceProvider.GetRequiredService<IOptions<FlexBusOptions>>().Value;
        _consumerOptions = serviceProvider.GetRequiredService<IOptions<ConsumerOptions>>().Value;
        _selector = serviceProvider.GetService<IConsumerServiceSelector>();
        _consumerClientFactory = serviceProvider.GetService<IConsumerClientFactory>();
        _serializer = serviceProvider.GetService<ISerializer>();
        _storage = serviceProvider.GetService<IDataStorage>();
        _dispatcher = serviceProvider.GetService<ISubscribeDispatcher>();
        _cts = new CancellationTokenSource();
    }

    public bool IsHealthy()
    {
        return _isHealthy;
    }

    public void Start()
    {
        var groupingMatches = _selector.SelectCandidates();

        foreach (var matchGroup in groupingMatches)
        {
            for (var i = 0; i < _consumerOptions.ThreadCount; i++)
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        using var client = _consumerClientFactory.Create(matchGroup);

                        _serverAddress = client.BrokerAddress;

                        RegisterMessageProcessor(client);

                        await client.Listening(_pollingDelay, _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        //ignore
                    }
                    catch (Exception e)
                    {
                        _isHealthy = false;
                        _logger.LogError(e, e.Message);
                    }
                }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
        _compositeTask = Task.CompletedTask;
    }

    public void Restart(bool force = false)
    {
        if (!IsHealthy() || force)
        {
            Pulse();

            _cts = new CancellationTokenSource();
            _isHealthy = true;

            Start();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            Pulse();

            _compositeTask?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException ex)
        {
            var innerEx = ex.InnerExceptions[0];
            if (!(innerEx is OperationCanceledException))
            {
                _logger.ExpectedOperationCanceledException(innerEx);
            }
        }
    }

    public void Pulse()
    {
        _cts?.Cancel();
    }

    private void RegisterMessageProcessor(IConsumerClient client)
    {
        client.OnMessageReceived += async (sender, transportMessage) =>
        {
            _logger.MessageReceived(transportMessage.GetId(), transportMessage.GetName());

            long? tracingTimestamp = null;
            try
            {
                tracingTimestamp = TracingBefore(transportMessage, _serverAddress);

                var name = transportMessage.GetName();
                var group = transportMessage.GetGroup();

                Message message;

                var executor = _selector.SelectBestCandidate(name);
                try
                {
                    if (executor == null)
                    {
                        var error = $"Message can not be found subscriber. Name:{name}, Group:{group}. {Environment.NewLine} see: https://github.com/dotnetcore/CAP/issues/63";
                        var ex = new SubscriberNotFoundException(error);

                        TracingError(tracingTimestamp, transportMessage, client.BrokerAddress, ex);

                        throw ex;
                    }
                    
                    message = await _serializer.DeserializeAsync(transportMessage, executor.MessageType);
                    message.RemoveException();
                }
                catch (Exception e)
                {
                    transportMessage.Headers.Add(Headers.Exception, nameof(SerializationException) + "-->" + e.Message);
                    if (transportMessage.Headers.TryGetValue(Headers.Type, out var val))
                    {
                        var dataUri = $"data:{val};base64," + Convert.ToBase64String(transportMessage.Body!);
                        message = new Message(transportMessage.Headers, dataUri);
                    }
                    else
                    {
                        var dataUri = "data:UnknownType;base64," + Convert.ToBase64String(transportMessage.Body!);
                        message = new Message(transportMessage.Headers, dataUri);
                    }
                }

                if (message.HasException())
                {
                    var content = _serializer.Serialize(message);

                    await _storage.StoreReceivedExceptionMessage(name, group, content);

                    client.Commit(sender);

                    try
                    {
                        _consumerOptions.FailedThresholdCallback?.Invoke(new FailedInfo
                        {
                            ServiceProvider = _serviceProvider,
                            MessageType = MessageType.Subscribe,
                            Message = message
                        });

                        _logger.ConsumerExecutedAfterThreshold(message.GetId(), _options.FailedRetryCount);
                    }
                    catch (Exception e)
                    {
                        _logger.ExecutedThresholdCallbackFailed(e);
                    }

                    TracingAfter(tracingTimestamp, transportMessage, _serverAddress);
                }
                else
                {
                    client.Commit(sender);

                    var mediumMessage = await _storage.StoreReceivedMessage(name, group, message);
                    mediumMessage.Origin = message;

                    TracingAfter(tracingTimestamp, transportMessage, _serverAddress);

                    await _dispatcher.DispatchAsync(mediumMessage, executor);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occurred when process received message. Message:'{0}'.", transportMessage);

                client.Reject(sender);

                TracingError(tracingTimestamp, transportMessage, client.BrokerAddress, e);
            }
        };

        client.OnLog += WriteLog;
    }

    private void WriteLog(object sender, LogMessageEventArgs logMsg)
    {
        switch (logMsg.LogType)
        {
            case MqLogType.ConsumerCancelled:
                _logger.LogWarning("RabbitMQ consumer cancelled. --> " + logMsg.Reason);
                break;
            case MqLogType.ConsumerRegistered:
                _logger.LogInformation("RabbitMQ consumer registered. --> " + logMsg.Reason);
                break;
            case MqLogType.ConsumerUnregistered:
                _logger.LogWarning("RabbitMQ consumer unregistered. --> " + logMsg.Reason);
                break;
            case MqLogType.ConsumerShutdown:
                _isHealthy = false;
                _logger.LogWarning("RabbitMQ consumer shutdown. --> " + logMsg.Reason);
                break;
            case MqLogType.ConsumeError:
                _logger.LogError("Kafka client consume error. --> " + logMsg.Reason);
                break;
            case MqLogType.ServerConnError:
                _isHealthy = false;
                _logger.LogCritical("Kafka server connection error. --> " + logMsg.Reason);
                break;
            case MqLogType.ExceptionReceived:
                _logger.LogError("AzureServiceBus subscriber received an error. --> " + logMsg.Reason);
                break;
            case MqLogType.InvalidIdFormat:
                _logger.LogError("AmazonSQS subscriber delete inflight message failed, invalid id. --> " + logMsg.Reason);
                break;
            case MqLogType.MessageNotInflight:
                _logger.LogError("AmazonSQS subscriber change message's visibility failed, message isn't in flight. --> " + logMsg.Reason);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private long? TracingBefore(TransportMessage message, BrokerAddress broker)
    {
        if (s_diagnosticListener.IsEnabled(CapDiagnosticListenerNames.BeforeConsume))
        {
            var eventData = new CapEventDataSubStore()
            {
                OperationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Operation = message.GetName(),
                BrokerAddress = broker,
                TransportMessage = message
            };

            s_diagnosticListener.Write(CapDiagnosticListenerNames.BeforeConsume, eventData);

            return eventData.OperationTimestamp;
        }

        return null;
    }

    private void TracingAfter(long? tracingTimestamp, TransportMessage message, BrokerAddress broker)
    {
        if (tracingTimestamp != null && s_diagnosticListener.IsEnabled(CapDiagnosticListenerNames.AfterConsume))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var eventData = new CapEventDataSubStore()
            {
                OperationTimestamp = now,
                Operation = message.GetName(),
                BrokerAddress = broker,
                TransportMessage = message,
                ElapsedTimeMs = now - tracingTimestamp.Value
            };

            s_diagnosticListener.Write(CapDiagnosticListenerNames.AfterConsume, eventData);
        }
    }

    private void TracingError(long? tracingTimestamp, TransportMessage message, BrokerAddress broker, Exception ex)
    {
        if (tracingTimestamp != null && s_diagnosticListener.IsEnabled(CapDiagnosticListenerNames.ErrorConsume))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var eventData = new CapEventDataSubStore()
            {
                OperationTimestamp = now,
                Operation = message.GetName(),
                BrokerAddress = broker,
                TransportMessage = message,
                ElapsedTimeMs = now - tracingTimestamp.Value,
                Exception = ex
            };

            s_diagnosticListener.Write(CapDiagnosticListenerNames.ErrorConsume, eventData);
        }
    }
}