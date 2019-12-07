using System;

namespace HtcSharp.HttpModule2.Configuration {
    public interface IChangeToken {
        bool HasChanged { get; }
        bool ActiveChangeCallbacks { get; }
        IDisposable RegisterChangeCallback(Action<object> callback, object state);
    }
}