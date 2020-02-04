using System;
using HtcSharp.HttpModule.Infrastructure.Options;
using Microsoft.Extensions.Configuration;

namespace HtcSharp.HttpModule.Infrastructure.Configuration {
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
