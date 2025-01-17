﻿using FlexBus.Transport;
using Microsoft.Extensions.Logging;

namespace FlexBus.Test.FakeInMemoryQueue
{
    internal sealed class InMemoryConsumerClientFactory : IConsumerClientFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly InMemoryQueue _queue;

        public InMemoryConsumerClientFactory(ILoggerFactory loggerFactory, InMemoryQueue queue)
        {
            _loggerFactory = loggerFactory;
            _queue = queue;
        }

        public IConsumerClient Create(string groupId)
        {
            var logger = _loggerFactory.CreateLogger(typeof(InMemoryConsumerClient));
            return new InMemoryConsumerClient(logger, _queue, groupId);
        }
    }
}