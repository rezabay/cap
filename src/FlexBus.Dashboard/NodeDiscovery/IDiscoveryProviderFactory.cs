﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace FlexBus.Dashboard.NodeDiscovery
{
    internal interface IDiscoveryProviderFactory
    {
        INodeDiscoveryProvider Create(DiscoveryOptions options);
    }
}