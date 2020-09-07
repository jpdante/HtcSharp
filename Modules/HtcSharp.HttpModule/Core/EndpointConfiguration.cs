// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;

namespace HtcSharp.HttpModule.Core {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\EndpointConfiguration.cs
    // Start-At-Remote-Line 10
    // SourceTools-End
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