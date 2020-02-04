using HtcSharp.HttpModule.Infrastructure.Heartbeat;
using HtcSharp.HttpModule.Infrastructure.Interfaces;

namespace HtcSharp.HttpModule.Infrastructure.Extensions {
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