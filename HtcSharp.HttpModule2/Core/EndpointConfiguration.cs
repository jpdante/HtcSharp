using System;
using HtcSharp.HttpModule2.Connection.ListenOptions;

namespace HtcSharp.HttpModule2.Core {
    public class EndpointConfiguration {
        internal EndpointConfiguration(bool isHttps, ListenOptions listenOptions, HttpsConnectionAdapterOptions httpsOptions) {
            IsHttps = isHttps;
            ListenOptions = listenOptions ?? throw new ArgumentNullException(nameof(listenOptions));
            HttpsOptions = httpsOptions ?? throw new ArgumentNullException(nameof(httpsOptions));
        }

        public bool IsHttps { get; }
        public ListenOptions ListenOptions { get; }
        public HttpsConnectionAdapterOptions HttpsOptions { get; }
    }
}