using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HtcSharp.HttpModule.Core.Http.Http2 {
    internal class ThreadPoolAwaitable : ICriticalNotifyCompletion {
        public static ThreadPoolAwaitable Instance = new ThreadPoolAwaitable();

        private ThreadPoolAwaitable() {
        }

        public ThreadPoolAwaitable GetAwaiter() => this;
        public bool IsCompleted => false;

        public void GetResult() {
        }

        public void OnCompleted(Action continuation) {
            ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), continuation);
        }

        public void UnsafeOnCompleted(Action continuation) {
            OnCompleted(continuation);
        }
    }
}