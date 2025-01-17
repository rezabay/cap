﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace FlexBus.Test
{
    public class CapBuilderTest
    {

        [Fact]
        public void CanCreateInstanceAndGetService()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IFlexBusPublisher, MyProducerService>();
            var builder = new FlexBusBuilder(services);
            Assert.NotNull(builder);

            var count = builder.Services.Count;
            Assert.Equal(1, count);

            var provider = services.BuildServiceProvider();
            var capPublisher = provider.GetService<IFlexBusPublisher>();
            Assert.NotNull(capPublisher);
        }

        [Fact]
        public void CanAddCapService()
        {
            var services = new ServiceCollection();
            services.AddFlexBus(x => { });
            var builder = services.BuildServiceProvider();

            var markService = builder.GetService<CapMarkerService>();
            Assert.NotNull(markService);
        }

        [Fact]
        public void CanOverridePublishService()
        {
            var services = new ServiceCollection();
            services.AddFlexBus(x => { }).AddProducerService<MyProducerService>();

            var thingy = services.BuildServiceProvider()
                .GetRequiredService<IFlexBusPublisher>() as MyProducerService;

            Assert.NotNull(thingy);
        }
      

        [Fact]
        public void CanResolveCapOptions()
        {
            var services = new ServiceCollection();
            services.AddFlexBus(x => { });
            var builder = services.BuildServiceProvider();
            var capOptions = builder.GetService<IOptions<FlexBusOptions>>().Value;
            Assert.NotNull(capOptions);
        }

        private class MyProducerService : IFlexBusPublisher
        {
            public IServiceProvider ServiceProvider { get; }

            public AsyncLocal<ICapTransaction> Transaction { get; }

            public Task PublishAsync<T>(string name, T contentObj, string callbackName = null,
                CancellationToken cancellationToken = default, DateTime? scheduleDate = null)
            {
                throw new NotImplementedException();
            }

            public Task PublishAsync<T>(string name, T contentObj, IDictionary<string, string> optionHeaders = null,
                CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public void Publish<T>(string name, T contentObj, string callbackName = null, DateTime? scheduleDate = null)
            {
                throw new NotImplementedException();
            }

            public void Publish<T>(string name, T contentObj, IDictionary<string, string> headers)
            {
                throw new NotImplementedException();
            }
        }
    }
}