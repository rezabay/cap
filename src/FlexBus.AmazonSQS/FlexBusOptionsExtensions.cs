﻿using System;
using Amazon;
using FlexBus;
using FlexBus.AmazonSQS;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CapOptionsExtensions
{
    public static FlexBusOptions UseAmazonSQS(this FlexBusOptions options, RegionEndpoint region)
    {
        return options.UseAmazonSQS(opt => { opt.Region =  region; });
    }

    public static FlexBusOptions UseAmazonSQS(this FlexBusOptions options, Action<AmazonSQSOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        options.RegisterExtension(new AmazonSQSOptionsExtension(configure));

        return options;
    }
}