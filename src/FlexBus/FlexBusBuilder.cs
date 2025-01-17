﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus;

/// <summary>
/// Used to verify cap service was called on a ServiceCollection
/// </summary>
public class CapMarkerService
{
}

/// <summary>
/// Used to verify cap storage extension was added on a ServiceCollection
/// </summary>
public class CapStorageMarkerService
{
}

/// <summary>
/// Used to verify cap message queue extension was added on a ServiceCollection
/// </summary>
public class CapMessageQueueMakerService
{
}

/// <summary>
/// Allows fine grained configuration of CAP services.
/// </summary>
public sealed class FlexBusBuilder
{
    public FlexBusBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Gets the <see cref="IServiceCollection" /> where MVC services are configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Add an <see cref="IFlexBusPublisher" />.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public FlexBusBuilder AddProducerService<T>()
        where T : class, IFlexBusPublisher
    {
        return AddScoped(typeof(IFlexBusPublisher), typeof(T));
    }

    /// <summary>
    /// Adds a scoped service of the type specified in serviceType with an implementation
    /// </summary>
    private FlexBusBuilder AddScoped(Type serviceType, Type concreteType)
    {
        Services.AddScoped(serviceType, concreteType);
        return this;
    }

    /// <summary>
    /// Adds a singleton service of the type specified in serviceType with an implementation
    /// </summary>
    private FlexBusBuilder AddSingleton(Type serviceType, Type concreteType)
    {
        Services.AddSingleton(serviceType, concreteType);
        return this;
    }
}