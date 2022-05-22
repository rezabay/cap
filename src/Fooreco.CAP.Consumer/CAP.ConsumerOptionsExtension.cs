﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP.Consumer.Internal;
using Fooreco.CAP.Consumer.Processor;
using Fooreco.CAP.Internal;
using Fooreco.CAP.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fooreco.CAP.Consumer
{
    internal sealed class ConsumerOptionsExtension : ICapOptionsExtension
    {
        internal static IServiceCollection ServiceCollection;
        private readonly Action<ConsumerOptions> _configure;


        public ConsumerOptionsExtension(Action<ConsumerOptions> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public void AddServices(IServiceCollection services)
        {
            ServiceCollection = services;

            services.Configure(_configure);

            services.TryAddSingleton<IConsumerServiceSelector, ConsumerServiceSelector>();
            services.TryAddSingleton<ISubscribeInvoker, SubscribeInvoker>();
            services.TryAddSingleton<MethodMatcherCache>();

            services.TryAddSingleton<IConsumerRegister, ConsumerRegister>();
            services.TryAddSingleton<ISubscribeDispatcher, SubscribeDispatcher>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessor, TransportCheckProcessor>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, ConsumerRegister>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessor, CollectorProcessor>());
        }
    }
}
