using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using HtcSharp.HttpModule2.Connection.ListenOptions;
using HtcSharp.HttpModule2.Cryptography;

namespace HtcSharp.HttpModule2.Core {
    public class HtcServerOptions {

        internal List<ListenOptions> ListenOptions { get; } = new List<ListenOptions>();

        public bool AddServerHeader { get; set; } = true;

        public bool AllowSynchronousIO { get; set; } = false;

        public bool DisableStringReuse { get; set; } = false;

        public IServiceProvider ApplicationServices { get; set; }

        public HtcServerLimits Limits { get; } = new HtcServerLimits();

        public HtcConfigurationLoader ConfigurationLoader { get; set; }

        private Action<ListenOptions> EndpointDefaults { get; set; } = _ => { };

        private Action<HttpsConnectionAdapterOptions> HttpsDefaults { get; set; } = _ => { };

        internal X509Certificate2 DefaultCertificate { get; set; }

        internal bool IsDevCertLoaded { get; set; }

        public void ConfigureEndpointDefaults(Action<ListenOptions> configureOptions) {
            EndpointDefaults = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        internal void ApplyEndpointDefaults(ListenOptions listenOptions) {
            listenOptions.KestrelServerOptions = this;
            ConfigurationLoader?.ApplyConfigurationDefaults(listenOptions);
            EndpointDefaults(listenOptions);
        }

        public void ConfigureHttpsDefaults(Action<HttpsConnectionAdapterOptions> configureOptions) {
            HttpsDefaults = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        internal void ApplyHttpsDefaults(HttpsConnectionAdapterOptions httpsOptions) {
            HttpsDefaults(httpsOptions);
        }

        internal void ApplyDefaultCert(HttpsConnectionAdapterOptions httpsOptions) {
            if (httpsOptions.ServerCertificate != null || httpsOptions.ServerCertificateSelector != null) {
                return;
            }

            EnsureDefaultCert();

            httpsOptions.ServerCertificate = DefaultCertificate;
        }

        private void EnsureDefaultCert() {
            // TODO: Fix EnsureDefaultCert
            /*if (DefaultCertificate == null && !IsDevCertLoaded) {
                IsDevCertLoaded = true; // Only try once
                var logger = ApplicationServices.GetRequiredService<ILogger<KestrelServer>>();
                try {
                    DefaultCertificate = CertificateManager.ListCertificates(CertificatePurpose.HTTPS, StoreName.My, StoreLocation.CurrentUser, isValid: true).FirstOrDefault();

                    if (DefaultCertificate != null) {
                        logger.LocatedDevelopmentCertificate(DefaultCertificate);
                    } else {
                        logger.UnableToLocateDevelopmentCertificate();
                    }
                } catch {
                    logger.UnableToLocateDevelopmentCertificate();
                }
            }*/
        }

        public HtcConfigurationLoader Configure() {
            var loader = new HtcConfigurationLoader(this);
            ConfigurationLoader = loader;
            return loader;
        }

        public void Listen(IPAddress address, int port) {
            Listen(address, port, _ => { });
        }

        public void Listen(IPAddress address, int port, Action<ListenOptions> configure) {
            if (address == null) {
                throw new ArgumentNullException(nameof(address));
            }

            Listen(new IPEndPoint(address, port), configure);
        }

        public void Listen(IPEndPoint endPoint) {
            Listen(endPoint, _ => { });
        }

        public void Listen(IPEndPoint endPoint, Action<ListenOptions> configure) {
            if (endPoint == null) {
                throw new ArgumentNullException(nameof(endPoint));
            }
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new ListenOptions(endPoint);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        public void ListenLocalhost(int port) => ListenLocalhost(port, options => { });

        public void ListenLocalhost(int port, Action<ListenOptions> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new LocalhostListenOptions(port);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        public void ListenAnyIP(int port) => ListenAnyIP(port, options => { });

        public void ListenAnyIP(int port, Action<ListenOptions> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new AnyIPListenOptions(port);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        public void ListenUnixSocket(string socketPath) {
            ListenUnixSocket(socketPath, _ => { });
        }

        public void ListenUnixSocket(string socketPath, Action<ListenOptions> configure) {
            if (socketPath == null) {
                throw new ArgumentNullException(nameof(socketPath));
            }

            if (!Path.IsPathRooted(socketPath)) {
                throw new ArgumentException("Unix socket path must be absolute.", nameof(socketPath));
            }
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new ListenOptions(socketPath);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        public void ListenHandle(ulong handle) {
            ListenHandle(handle, _ => { });
        }

        public void ListenHandle(ulong handle, Action<ListenOptions> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new ListenOptions(handle);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }
    }
}