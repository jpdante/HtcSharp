using HtcSharp.HttpModule.Core.Http.Http2;

namespace HtcSharp.HttpModule.Core.Infrastructure {
    internal interface ITimeoutControl {
        TimeoutReason TimerReason { get; }

        void SetTimeout(long ticks, TimeoutReason timeoutReason);
        void ResetTimeout(long ticks, TimeoutReason timeoutReason);
        void CancelTimeout();

        void InitializeHttp2(InputFlowControl connectionInputFlowControl);
        void StartRequestBody(MinDataRate minRate);
        void StopRequestBody();
        void StartTimingRead();
        void StopTimingRead();
        void BytesRead(long count);

        void StartTimingWrite();
        void StopTimingWrite();
        void BytesWrittenToBuffer(MinDataRate minRate, long count);
    }
}
