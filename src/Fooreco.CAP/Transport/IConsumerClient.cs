﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Fooreco.CAP.Messages;
using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fooreco.CAP.Transport
{
    /// <inheritdoc />
    /// <summary>
    /// Message queue consumer client
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        event EventHandler<TransportMessage> OnMessageReceived;

        event EventHandler<LogMessageEventArgs> OnLog;

        BrokerAddress BrokerAddress { get; }

        /// <summary>
        /// Start listening
        /// </summary>
        Task Listening(TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        void Commit([NotNull] object sender);

        /// <summary>
        /// Reject message and resumption
        /// </summary>
        void Reject([CanBeNull] object sender);
    }
}