// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Infrastructure {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Infrastructure\TimeoutControlExtensions.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal static class TimeoutControlExtensions {
        public static void StartDrainTimeout(this ITimeoutControl timeoutControl, MinDataRate minDataRate, long? maxResponseBufferSize) {
            // If maxResponseBufferSize has no value, there's no backpressure and we can't reasonably time out draining.
            if (minDataRate == null || maxResponseBufferSize == null) {
                return;
            }

            // Ensure we have at least the grace period from this point to finish draining the response.
            timeoutControl.BytesWrittenToBuffer(minDataRate, 1);
            timeoutControl.StartTimingWrite();
        }
    }
}