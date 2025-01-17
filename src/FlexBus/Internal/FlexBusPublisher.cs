﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Diagnostics;
using FlexBus.Messages;
using FlexBus.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.Internal;

internal class FlexBusPublisher : IFlexBusPublisher
{
    private readonly IDataStorage _storage;

    // ReSharper disable once InconsistentNaming
    private static readonly DiagnosticListener s_diagnosticListener = 
        new (CapDiagnosticListenerNames.DiagnosticListenerName);

    public FlexBusPublisher(IServiceProvider service)
    {
        ServiceProvider = service;
        _storage = service.GetRequiredService<IDataStorage>();
        Transaction = new AsyncLocal<ICapTransaction>();
    }

    public IServiceProvider ServiceProvider { get; }

    public AsyncLocal<ICapTransaction> Transaction { get; }

    public Task PublishAsync<T>(string name, T value, 
        IDictionary<string, string> headers, 
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Publish(name, value, headers), cancellationToken);
    }

    public Task PublishAsync<T>(string name, T value, 
        string callbackName = null,
        CancellationToken cancellationToken = default,
        DateTime? scheduleDate = null)
    {
        return Task.Run(() => Publish(name, value, callbackName, scheduleDate), cancellationToken);
    }

    public void Publish<T>(string name, T value, string callbackName = null, DateTime? scheduleDate = null)
    {
        var header = new Dictionary<string, string>
        {
            {Headers.CallbackName, callbackName}
        };
        if (scheduleDate != null)
        {
            header.Add(Headers.ScheduleDate, scheduleDate.ToString());
        }

        Publish(name, value, header);
    }

    public void Publish<T>(string name, 
        T value, 
        IDictionary<string, string> headers)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        headers ??= new Dictionary<string, string>();

        if (!headers.ContainsKey(Headers.MessageId))
        {
            var messageId = SnowflakeId.Default().NextId().ToString();
            headers.Add(Headers.MessageId, messageId);
        }
             
        if (!headers.ContainsKey(Headers.CorrelationId))
        {
            headers.Add(Headers.CorrelationId, headers[Headers.MessageId]);
            headers.Add(Headers.CorrelationSequence, 0.ToString());
        }
        headers.Add(Headers.MessageName, name);
        headers.Add(Headers.Type, typeof(T).Name);
        headers.Add(Headers.SentTime, DateTimeOffset.Now.ToString());

        var message = new Message(headers, value);

        long? tracingTimestamp = null;
        try
        {
            tracingTimestamp = TracingBefore(message);

            if (Transaction.Value?.DbTransaction == null)
            {
                _storage.StoreMessage(name, message);

                TracingAfter(tracingTimestamp, message);
            }
            else
            {
                var transaction = (FlexBusTransactionBase)Transaction.Value;

                _storage.StoreMessage(name, message, transaction.DbTransaction);

                TracingAfter(tracingTimestamp, message);

                if (transaction.AutoCommit)
                {
                    transaction.Commit();
                }
            }
        }
        catch (Exception e)
        {
            TracingError(tracingTimestamp, message, e);
            throw;
        }
    }

    private long? TracingBefore(Message message)
    {
        if (s_diagnosticListener.IsEnabled(CapDiagnosticListenerNames.BeforePublishMessageStore))
        {
            var eventData = new CapEventDataPubStore()
            {
                OperationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Operation = message.GetName(),
                Message = message
            };

            s_diagnosticListener.Write(CapDiagnosticListenerNames.BeforePublishMessageStore, eventData);

            return eventData.OperationTimestamp;
        }

        return null;
    }

    private void TracingAfter(long? tracingTimestamp, Message message)
    {
        if (tracingTimestamp != null && s_diagnosticListener.IsEnabled(CapDiagnosticListenerNames.AfterPublishMessageStore))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var eventData = new CapEventDataPubStore()
            {
                OperationTimestamp = now,
                Operation = message.GetName(),
                Message = message,
                ElapsedTimeMs = now - tracingTimestamp.Value
            };

            s_diagnosticListener.Write(CapDiagnosticListenerNames.AfterPublishMessageStore, eventData);
        }
    }

    private void TracingError(long? tracingTimestamp, Message message, Exception ex)
    {
        if (tracingTimestamp != null && s_diagnosticListener.IsEnabled(CapDiagnosticListenerNames.ErrorPublishMessageStore))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var eventData = new CapEventDataPubStore()
            {
                OperationTimestamp = now,
                Operation = message.GetName(),
                Message = message,
                ElapsedTimeMs = now - tracingTimestamp.Value,
                Exception = ex
            };

            s_diagnosticListener.Write(CapDiagnosticListenerNames.ErrorPublishMessageStore, eventData);
        }
    }
}