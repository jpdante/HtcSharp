using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.HttpModule.Infrastructure.Features;
using HtcSharp.HttpModule.Infrastructure.Heart;

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
