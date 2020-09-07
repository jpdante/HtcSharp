// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Shared.ValueTaskExtensions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Shared\ValueTaskExtensions\ValueTaskExtensions.cs
    // Start-At-Remote-Line 9
    // SourceTools-End
    internal static class ValueTaskExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task GetAsTask(this in ValueTask<FlushResult> valueTask) {
            // Try to avoid the allocation from AsTask
            if (valueTask.IsCompletedSuccessfully) {
                // Signal consumption to the IValueTaskSource
                valueTask.GetAwaiter().GetResult();
                return Task.CompletedTask;
            } else {
                return valueTask.AsTask();
            }
        }
    }
}