﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP;
using Fooreco.CAP.Internal;
using Fooreco.CAP.Processor;
using Fooreco.CAP.Serialization;
using Fooreco.CAP.Transport;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection" /> for configuring consistence services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures the consistence services for the consistency.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="CapOptions" />.</param>
        /// <returns>An <see cref="CapBuilder" /> for application services.</returns>
        public static CapBuilder AddCap(this IServiceCollection services, Action<CapOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.TryAddSingleton<CapMarkerService>();
            services.TryAddSingleton<ICapPublisher, CapPublisher>();

            // Processors
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, CapProcessingServer>());

            // Sender and Executors
            services.TryAddSingleton<IDispatcher, Dispatcher>();

            services.TryAddSingleton<ISerializer, JsonUtf8Serializer>();

            // Options and extension service
            var options = new CapOptions();
            setupAction(options);
            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }
            services.Configure(setupAction);

            // Startup and Hosted 
            services.AddSingleton<IBootstrapper, Bootstrapper>();
            services.AddHostedService<Bootstrapper>();

            return new CapBuilder(services);
        }
    }
}