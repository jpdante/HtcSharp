using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.HttpModule.Core.Http.Features;
using HtcSharp.HttpModule.Infrastructure.Heartbeat;

namespace HtcSharp.HttpModule.Core.Http.Http {
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
