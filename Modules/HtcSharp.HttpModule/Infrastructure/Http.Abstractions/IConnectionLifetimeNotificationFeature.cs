using System.Threading;

namespace HtcSharp.HttpModule.Infrastructure.Http.Abstractions {
    public interface IConnectionLifetimeNotificationFeature {
        CancellationToken ConnectionClosedRequested { get; set; }

        void RequestClose();
    }
}
