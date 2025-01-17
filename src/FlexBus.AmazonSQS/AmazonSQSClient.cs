﻿using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using FlexBus;

namespace FlexBus.AmazonSQS
{
    internal class AmazonSQSClientWrapper
    {
        private static readonly SemaphoreSlim ConnectionLock = new(initialCount: 1, maxCount: 1);

        protected readonly AmazonSQSOptions SQSOptions;
        protected readonly FlexBusOptions FlexBusOptions;

        protected IAmazonSQS SQSClient;
        protected string QueueUrl = string.Empty;
        protected string QueueName;

        protected AmazonSQSClientWrapper(IOptions<AmazonSQSOptions> sqsOptions,
                                         IOptions<FlexBusOptions> capOptions)
        {
            SQSOptions = sqsOptions.Value;
            FlexBusOptions = capOptions.Value;
        }

        protected async Task ConnectToSQS(string queueName = null)
        {
            if (SQSClient != null)
            {
                return;
            }

            ConnectionLock.Wait();

            try
            {
                var config = new AmazonSQSConfig()
                {
                    RegionEndpoint = SQSOptions.Region,
                    Timeout = TimeSpan.FromSeconds(10),
                    RetryMode = RequestRetryMode.Standard,
                    MaxErrorRetry = 3
                };
                SQSClient = SQSOptions.Credentials != null
                    ? new AmazonSQSClient(SQSOptions.Credentials, config)
                    : new AmazonSQSClient(config);

                QueueName = string.IsNullOrEmpty(queueName) 
                    ? FlexBusOptions.DefaultGroup + "." + FlexBusOptions.Version 
                    : queueName;
                var queue = await SQSClient.CreateQueueAsync(QueueName.NormalizeForAws());
                QueueUrl = queue.QueueUrl;
            }
            finally
            {
                ConnectionLock.Release();
            }
        }
    }
}
