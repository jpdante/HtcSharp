// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Features;
using HtcSharp.HttpModule.Infrastructure;

namespace HtcSharp.HttpModule.Http.Protocols.Http {
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
