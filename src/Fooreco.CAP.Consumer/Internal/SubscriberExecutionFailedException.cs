﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Fooreco.CAP.Consumer.Internal
{
    internal class SubscriberExecutionFailedException : Exception
    {
        public SubscriberExecutionFailedException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
