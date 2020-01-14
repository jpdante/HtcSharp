using System.Threading;

namespace HtcSharp.HttpModule.Http.Http.Abstractions {
    public interface IConnectionLifetimeNotificationFeature {
        CancellationToken ConnectionClosedRequested { get; set; }

        void RequestClose();
    }
}
