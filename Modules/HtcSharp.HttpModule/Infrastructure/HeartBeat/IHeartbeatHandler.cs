using System;

namespace HtcSharp.HttpModule.Infrastructure.HeartBeat {
    internal interface IHeartbeatHandler {
        void OnHeartbeat(DateTimeOffset now);
    }
}
