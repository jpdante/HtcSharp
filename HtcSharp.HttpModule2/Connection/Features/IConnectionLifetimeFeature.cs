using System.Threading;

namespace HtcSharp.HttpModule2.Connection.Features {
    public interface IConnectionLifetimeFeature {
        CancellationToken ConnectionClosed { get; set; }
        void Abort();
    }
}