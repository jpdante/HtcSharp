using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule2.Configuration;
using HtcSharp.HttpModule2.Connection.Address;
using HtcSharp.HttpModule2.Connection.ListenOptions;
using HtcSharp.HttpModule2.Cryptography;

namespace HtcSharp.HttpModule2.Core {
    public class HtcConfigurationLoader {
        private bool _loaded = false;

        internal HtcConfigurationLoader(HtcServerOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public HtcServerOptions Options { get; }
        private IDictionary<string, Action<EndpointConfiguration>> EndpointConfigurations { get; } = new Dictionary<string, Action<EndpointConfiguration>>(0, StringComparer.OrdinalIgnoreCase);
        private IList<Action> EndpointsToAdd { get; } = new List<Action>();

        public HtcConfigurationLoader Endpoint(string name, Action<EndpointConfiguration> configureOptions) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            EndpointConfigurations[name] = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            return this;
        }

        public HtcConfigurationLoader Endpoint(IPAddress address, int port) => Endpoint(address, port, _ => { });

        public HtcConfigurationLoader Endpoint(IPAddress address, int port, Action<ListenOptions> configure) {
            if (address == null) {
                throw new ArgumentNullException(nameof(address));
            }

            return Endpoint(new IPEndPoint(address, port), configure);
        }

        public HtcConfigurationLoader Endpoint(IPEndPoint endPoint) => Endpoint(endPoint, _ => { });

        public HtcConfigurationLoader Endpoint(IPEndPoint endPoint, Action<ListenOptions> configure) {
            if (endPoint == null) {
                throw new ArgumentNullException(nameof(endPoint));
            }
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            EndpointsToAdd.Add(() => {
                Options.Listen(endPoint, configure);
            });

            return this;
        }

        public HtcConfigurationLoader LocalhostEndpoint(int port) => LocalhostEndpoint(port, options => { });

        public HtcConfigurationLoader LocalhostEndpoint(int port, Action<ListenOptions> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            EndpointsToAdd.Add(() => {
                Options.ListenLocalhost(port, configure);
            });

            return this;
        }

        public HtcConfigurationLoader AnyIPEndpoint(int port) => AnyIPEndpoint(port, options => { });

        public HtcConfigurationLoader AnyIPEndpoint(int port, Action<ListenOptions> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            EndpointsToAdd.Add(() => {
                Options.ListenAnyIP(port, configure);
            });

            return this;
        }

        public HtcConfigurationLoader UnixSocketEndpoint(string socketPath) => UnixSocketEndpoint(socketPath, _ => { });

        public HtcConfigurationLoader UnixSocketEndpoint(string socketPath, Action<ListenOptions> configure) {
            if (socketPath == null) {
                throw new ArgumentNullException(nameof(socketPath));
            }
            if (socketPath.Length == 0 || socketPath[0] != '/') {
                throw new ArgumentException("Unix socket path must be absolute.", nameof(socketPath));
            }
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            EndpointsToAdd.Add(() => {
                Options.ListenUnixSocket(socketPath, configure);
            });

            return this;
        }

        public HtcConfigurationLoader HandleEndpoint(ulong handle) => HandleEndpoint(handle, _ => { });

        public HtcConfigurationLoader HandleEndpoint(ulong handle, Action<ListenOptions> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            EndpointsToAdd.Add(() => {
                Options.ListenHandle(handle, configure);
            });

            return this;
        }

        internal void ApplyConfigurationDefaults(ListenOptions listenOptions) {
            var defaults = ConfigurationReader.EndpointDefaults;

            if (defaults.Protocols.HasValue) {
                listenOptions.Protocols = defaults.Protocols.Value;
            }
        }

        public void Load() {
            if (_loaded) {
                return;
            }
            _loaded = true;
            LoadDefaultCert(ConfigurationReader);
            foreach (var endpoint in ConfigurationReader.Endpoints) {
                var listenOptions = AddressBinder.ParseAddress(endpoint.Url, out var https);
                Options.ApplyEndpointDefaults(listenOptions);

                if (endpoint.Protocols.HasValue) {
                    listenOptions.Protocols = endpoint.Protocols.Value;
                }
                var httpsOptions = new HttpsConnectionAdapterOptions();
                if (https) {
                    Options.ApplyHttpsDefaults(httpsOptions);
                    httpsOptions.ServerCertificate = LoadCertificate(endpoint.Certificate, endpoint.Name) ?? httpsOptions.ServerCertificate;
                    Options.ApplyDefaultCert(httpsOptions);
                }

                if (EndpointConfigurations.TryGetValue(endpoint.Name, out var configureEndpoint)) {
                    var endpointConfig = new EndpointConfiguration(https, listenOptions, httpsOptions);
                    configureEndpoint(endpointConfig);
                }

                if (https && !listenOptions.IsTls) {
                    if (httpsOptions.ServerCertificate == null && httpsOptions.ServerCertificateSelector == null) {
                        throw new InvalidOperationException("Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.\nTo generate a developer certificate run 'dotnet dev-certs https'. To trust the certificate (Windows and macOS only) run 'dotnet dev-certs https --trust'.\nFor more information on configuring HTTPS see https://go.microsoft.com/fwlink/?linkid=848054.");
                    }
                    listenOptions.UseHttps(httpsOptions);
                }

                Options.ListenOptions.Add(listenOptions);
            }

            foreach (var action in EndpointsToAdd) {
                action();
            }
        }

        private void LoadDefaultCert(ConfigurationReader configReader) {
            if (configReader.Certificates.TryGetValue("Default", out var defaultCertConfig)) {
                var defaultCert = LoadCertificate(defaultCertConfig, "Default");
                if (defaultCert != null) {
                    Options.DefaultCertificate = defaultCert;
                }
            } else {
                var logger = Options.ApplicationServices.GetRequiredService<ILogger<HtcServer>>();
                var certificate = FindDeveloperCertificateFile(configReader, logger);
                if (certificate != null) {
                    logger.LocatedDevelopmentCertificate(certificate);
                    Options.DefaultCertificate = certificate;
                }
            }
        }

        private X509Certificate2 FindDeveloperCertificateFile(ConfigurationReader configReader, Logger logger) {
            string certificatePath = null;
            try {
                if (configReader.Certificates.TryGetValue("Development", out var certificateConfig) &&
                    certificateConfig.Path == null &&
                    certificateConfig.Password != null &&
                    TryGetCertificatePath(out certificatePath) &&
                    File.Exists(certificatePath)) {
                    var certificate = new X509Certificate2(certificatePath, certificateConfig.Password);
                    return IsDevelopmentCertificate(certificate) ? certificate : null;
                } else if (!File.Exists(certificatePath)) {
                    logger.Error($"Failed to locate development certificate file '{certificatePath}'");
                }
            } catch (CryptographicException) {
                logger.Error($"Failed to load development certificate '{certificatePath}'");
            }
            return null;
        }

        private bool IsDevelopmentCertificate(X509Certificate2 certificate) {
            if (!string.Equals(certificate.Subject, "CN=localhost", StringComparison.Ordinal)) {
                return false;
            }

            foreach (var ext in certificate.Extensions) {
                if (string.Equals(ext.Oid.Value, CertificateManager.AspNetHttpsOid, StringComparison.Ordinal)) {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetCertificatePath(out string path) {
            var hostingEnvironment = Options.ApplicationServices.GetRequiredService<IHostEnvironment>();
            var appName = hostingEnvironment.ApplicationName;

            var appData = Environment.GetEnvironmentVariable("APPDATA");
            var home = Environment.GetEnvironmentVariable("HOME");
            var basePath = appData != null ? Path.Combine(appData, "ASP.NET", "https") : null;
            basePath = basePath ?? (home != null ? Path.Combine(home, ".aspnet", "https") : null);
            path = basePath != null ? Path.Combine(basePath, $"{appName}.pfx") : null;
            return path != null;
        }

        private X509Certificate2 LoadCertificate(CertificateConfig certInfo, string endpointName) {
            if (certInfo.IsFileCert && certInfo.IsStoreCert) {
                throw new InvalidOperationException(CoreStrings.FormatMultipleCertificateSources(endpointName));
            } else if (certInfo.IsFileCert) {
                var env = Options.ApplicationServices.GetRequiredService<IHostEnvironment>();
                return new X509Certificate2(Path.Combine(env.ContentRootPath, certInfo.Path), certInfo.Password);
            } else if (certInfo.IsStoreCert) {
                return LoadFromStoreCert(certInfo);
            }
            return null;
        }

        private static X509Certificate2 LoadFromStoreCert(CertificateConfig certInfo) {
            var subject = certInfo.Subject;
            var storeName = string.IsNullOrEmpty(certInfo.Store) ? StoreName.My.ToString() : certInfo.Store;
            var location = certInfo.Location;
            var storeLocation = StoreLocation.CurrentUser;
            if (!string.IsNullOrEmpty(location)) {
                storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), location, ignoreCase: true);
            }
            var allowInvalid = certInfo.AllowInvalid ?? false;
            return CertificateLoader.LoadFromStoreCert(subject, storeName, storeLocation, allowInvalid);
        }
    }
}
