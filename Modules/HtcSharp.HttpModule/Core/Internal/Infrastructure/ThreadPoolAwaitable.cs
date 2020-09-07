// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HtcSharp.HttpModule.Core.Internal.Infrastructure {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Infrastructure\ThreadPoolAwaitable.cs
    // Start-At-Remote-Line 9
    // SourceTools-End
    internal class ThreadPoolAwaitable : ICriticalNotifyCompletion {
        public static ThreadPoolAwaitable Instance = new ThreadPoolAwaitable();

        private ThreadPoolAwaitable() {
        }

        public ThreadPoolAwaitable GetAwaiter() => this;
        public bool IsCompleted => false;

        public void GetResult() {
        }

        public void OnCompleted(Action continuation) {
            ThreadPool.UnsafeQueueUserWorkItem(state => ((Action) state)(), continuation);
        }

        public void UnsafeOnCompleted(Action continuation) {
            OnCompleted(continuation);
        }
    }
}