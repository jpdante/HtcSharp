using HtcSharp.HttpModule.Infrastructure.HeartBeat;
using HtcSharp.HttpModule.Infrastructure.Protocol.Features;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    internal partial class Http1Connection : IHttpMinRequestBodyDataRateFeature,
        IHttpMinResponseDataRateFeature {
        MinDataRate IHttpMinRequestBodyDataRateFeature.MinDataRate {
            get => MinRequestBodyDataRate;
            set => MinRequestBodyDataRate = value;
        }

        MinDataRate IHttpMinResponseDataRateFeature.MinDataRate {
            get => MinResponseDataRate;
            set => MinResponseDataRate = value;
        }
    }
}
