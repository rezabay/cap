﻿using System;
using System.Threading.Tasks;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Persistence;
using FlexBus.Serialization;
using FlexBus.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlexBus.Producer.Internal;

internal class MessageSender : IMessageSender
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly IDataStorage _dataStorage;
    private readonly ISerializer _serializer;
    private readonly ITransport _transport;
    private readonly IOptions<FlexBusOptions> _options;
    private readonly IOptions<ProducerOptions> _producerOptions;

    public MessageSender(
        ILogger<MessageSender> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _options = serviceProvider.GetService<IOptions<FlexBusOptions>>();
        _producerOptions = serviceProvider.GetService<IOptions<ProducerOptions>>();
        _dataStorage = serviceProvider.GetService<IDataStorage>();
        _serializer = serviceProvider.GetService<ISerializer>();
        _transport = serviceProvider.GetService<ITransport>();
    }

    public Task Connect()
    {
        return _transport.Connect();
    }

    public async Task<OperateResult> SendAsync(MediumMessage message)
    {
        bool retry;
        OperateResult result;
        do
        {
            var executedResult = await SendWithoutRetryAsync(message);
            result = executedResult.Item2;
            if (result == OperateResult.Success)
            {
                return result;
            }
            retry = executedResult.Item1;
        } while (retry);

        return result;
    }

    private async Task<(bool, OperateResult)> SendWithoutRetryAsync(MediumMessage message)
    {
        var transportMsg = await _serializer.SerializeAsync(message.Origin);

        var tracingTimestamp = TracingBefore(transportMsg);

        message.ExpiresAt = DateTime.Now.AddSeconds(_producerOptions.Value.SucceedMessageExpiredAfter);
        var result = await _dataStorage.ChangePublishStateAsync(message, 
            StatusName.Succeeded, 
            () => _transport.SendAsync(transportMsg));

        if (result.Succeeded)
        {
            TracingAfter(tracingTimestamp, transportMsg);

            return (false, OperateResult.Success);
        }
        else
        {
            TracingError(tracingTimestamp, transportMsg, result);

            var needRetry = await SetFailedState(message, result.Exception);

            return (needRetry, OperateResult.Failed(result.Exception));
        }
    }

    private async Task<bool> SetFailedState(MediumMessage message, Exception ex)
    {
        var needRetry = UpdateMessageForRetry(message);

        message.Origin.AddOrUpdateException(ex);
        message.ExpiresAt = message.Added.AddDays(15);

        await _dataStorage.ChangePublishStateAsync(message, StatusName.Failed);

        return needRetry;
    }

    private bool UpdateMessageForRetry(MediumMessage message)
    {
        var retries = ++message.Retries;
        var retryCount = Math.Min(_options.Value.FailedRetryCount, 3);
        if (retries >= retryCount)
        {
            if (retries == _options.Value.FailedRetryCount)
            {
                try
                {
                    _producerOptions.Value.FailedThresholdCallback?.Invoke(new FailedInfo
                    {
                        ServiceProvider = _serviceProvider,
                        MessageType = MessageType.Publish,
                        Message = message.Origin
                    });

                    _logger.SenderAfterThreshold(message.DbId, _options.Value.FailedRetryCount);
                }
                catch (Exception ex)
                {
                    _logger.ExecutedThresholdCallbackFailed(ex);
                }
            }
            return false;
        }

        _logger.SenderRetrying(message.DbId, retries);

        return true;
    }

    private long? TracingBefore(TransportMessage message)
    {
        _logger.LogInformation($"[Publishing] Message: {message.GetName()}");

        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private void TracingAfter(long? tracingTimestamp, TransportMessage message)
    {
        if (tracingTimestamp != null)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _logger.LogInformation($"[Published] Message: {message.GetName()}, ElapsedTimeMs = {now - tracingTimestamp.Value}");
        }
        else
        {
            _logger.LogInformation($"[Published] Message: {message.GetName()}");
        }
    }

    private void TracingError(long? tracingTimestamp, TransportMessage message, OperateResult result)
    {
        var ex = new PublisherSentFailedException(result.ToString(), result.Exception);

        if (tracingTimestamp != null)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _logger.LogInformation(ex, $"[Published with error] Message: {message.GetName()}, ElapsedTimeMs = {now - tracingTimestamp.Value}");
        }
        else
        {
            _logger.LogInformation(ex, $"[Published with error] Message: {message.GetName()}");
        }
    }
}