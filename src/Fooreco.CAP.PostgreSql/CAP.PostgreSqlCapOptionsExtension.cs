﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP.Persistence;
using Fooreco.CAP.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Fooreco.CAP
{
    internal class PostgreSqlCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<PostgreSqlOptions> _configure;

        public PostgreSqlCapOptionsExtension(Action<PostgreSqlOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapStorageMarkerService>();
            services.Configure(_configure);
            services.AddSingleton<IConfigureOptions<PostgreSqlOptions>, ConfigurePostgreSqlOptions>();

            services.AddSingleton<IDataStorage, PostgreSqlDataStorage>();
            services.AddSingleton<IStorageInitializer, PostgreSqlStorageInitializer>();
            services.AddTransient<ICapTransaction, PostgreSqlCapTransaction>();
        }
    }
}