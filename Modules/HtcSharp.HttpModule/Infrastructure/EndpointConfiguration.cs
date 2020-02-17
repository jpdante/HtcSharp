// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using HtcSharp.HttpModule.Options;

namespace HtcSharp.HttpModule.Infrastructure {
    public class EndpointConfiguration {
        internal EndpointConfiguration(bool isHttps, ListenOptions listenOptions, HttpsConnectionAdapterOptions httpsOptions, IConfigurationSection configSection) {
            IsHttps = isHttps;
            ListenOptions = listenOptions ?? throw new ArgumentNullException(nameof(listenOptions));
            HttpsOptions = httpsOptions ?? throw new ArgumentNullException(nameof(httpsOptions));
            ConfigSection = configSection ?? throw new ArgumentNullException(nameof(configSection));
        }

        public bool IsHttps { get; }
        public ListenOptions ListenOptions { get; }
        public HttpsConnectionAdapterOptions HttpsOptions { get; }
        public IConfigurationSection ConfigSection { get; }
    }
}
