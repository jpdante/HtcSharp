using System;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    internal interface IHeartbeatHandler {
        void OnHeartbeat(DateTimeOffset now);
    }
}