using System.Threading;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IConnectionLifetimeFeature {
        CancellationToken ConnectionClosed { get; set; }

        void Abort();
    }
}