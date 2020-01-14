using System;

namespace HtcSharp.HttpModule.Http.Http.Abstractions {
    public interface IConnectionHeartbeatFeature {
        void OnHeartbeat(Action<object> action, object state);
    }
}