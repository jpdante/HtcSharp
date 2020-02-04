using HtcSharp.HttpModule.Core.Http.Http2;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Infrastructure.Heartbeat;

namespace HtcSharp.HttpModule.Infrastructure.Interfaces {
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
