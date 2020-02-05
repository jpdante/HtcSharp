using System.IO.Pipelines;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IConnectionTransportFeature {
        IDuplexPipe Transport { get; set; }
    }
}