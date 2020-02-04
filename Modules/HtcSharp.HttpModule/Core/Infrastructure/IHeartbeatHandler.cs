using System;

namespace HtcSharp.HttpModule.Core.Infrastructure {
    internal interface IHeartbeatHandler {
        void OnHeartbeat(DateTimeOffset now);
    }
}
