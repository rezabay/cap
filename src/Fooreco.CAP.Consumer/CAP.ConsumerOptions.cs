﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


// ReSharper disable once CheckNamespace
using System;
using Fooreco.CAP.Messages;

namespace Fooreco.CAP.Consumer
{
    public class ConsumerOptions
    {
        public ConsumerOptions()
        {
            ThreadCount = 1;
            SucceedMessageExpiredAfter = 24 * 3600;
        }

        /// <summary>
        /// Sent or received succeed message after time span of due, then the message will be deleted at due time.
        /// Default is 24*3600 seconds.
        /// </summary>
        public int SucceedMessageExpiredAfter { get; set; }

        /// <summary>
        /// We’ll invoke this call-back with message type,name,content when retry failed (send or executed) messages equals <see cref="FailedRetryCount"/> times.
        /// </summary>
        public Action<FailedInfo> FailedThresholdCallback { get; set; }

        /// <summary>
        /// The number of consumer thread connections.
        /// Default is 1
        /// </summary>
        public int ThreadCount { get; set; }
    }
}
