using System;

namespace HtcSharp.HttpModule.Infrastructure.Http.Abstractions {
    public interface IConnectionHeartbeatFeature {
        void OnHeartbeat(Action<object> action, object state);
    }
}