using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Core.Http.Http2.FlowControl {
    internal class OutputFlowControlAwaitable : ICriticalNotifyCompletion {
        private static readonly Action _callbackCompleted = () => { };

        private Action _callback;

        public OutputFlowControlAwaitable GetAwaiter() => this;
        public bool IsCompleted => ReferenceEquals(_callback, _callbackCompleted);

        public void GetResult() {
            Debug.Assert(ReferenceEquals(_callback, _callbackCompleted));

            _callback = null;
        }

        public void OnCompleted(Action continuation) {
            if (ReferenceEquals(_callback, _callbackCompleted) ||
                ReferenceEquals(Interlocked.CompareExchange(ref _callback, continuation, null), _callbackCompleted)) {
                Task.Run(continuation);
            }
        }

        public void UnsafeOnCompleted(Action continuation) {
            OnCompleted(continuation);
        }

        public void Complete() {
            var continuation = Interlocked.Exchange(ref _callback, _callbackCompleted);

            continuation?.Invoke();
        }
    }
}
