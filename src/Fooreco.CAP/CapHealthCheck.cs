﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Fooreco.CAP.Transport;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fooreco.CAP
{
    public class CapHealthCheck : IHealthCheck
    {
        private readonly ITransport _transport;

        public CapHealthCheck(ITransport transaport)
        {
            _transport = transaport ?? throw new ArgumentNullException(nameof(transaport));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                if (await _transport.IsConnected())
                {
                    return HealthCheckResult.Healthy($"{_transport.BrokerAddress.Name} connection established.");
                }
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(status: context.Registration.FailureStatus,
                                             description: $"Unable to connect to {_transport.BrokerAddress.Name}.",
                                             exception: ex);
            }

            return HealthCheckResult.Unhealthy($"{_transport.BrokerAddress.Name} connection failure.");
        }
    }
}