using System;

namespace HtcSharp.HttpModule.Infrastructure.Interfaces {
    internal interface IHeartbeatHandler {
        void OnHeartbeat(DateTimeOffset now);
    }
}
