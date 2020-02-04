using HtcSharp.HttpModule.Core.Infrastructure;

namespace HtcSharp.HttpModule.Infrastructure.Interfaces {
    internal interface ITimeoutHandler {
        void OnTimeout(TimeoutReason reason);
    }
}
