using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Core.Exceptions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Logging;
using HtcSharp.HttpModule.Mvc;
using HtcSharp.HttpModule.Routing.Directives;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ILogger = HtcSharp.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace HtcSharp.HttpModule {
    public class HttpEngine {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private static HttpEngine _httpEngine;

        private readonly string _sitesPath;
        private readonly LogLevel _logLevel;
        private readonly SiteCollection _sites;
        private readonly DirectiveManager _directiveManager;

        private IWebHost _webHost;

        public HttpEngine(string sitesPath, string logLevel) {
            _sitesPath = sitesPath;
            if (!Enum.TryParse<LogLevel>(logLevel, out _logLevel)) {
                _logLevel = LogLevel.Warning;
            }
            _sites = new SiteCollection();
            _directiveManager = new DirectiveManager();
            _httpEngine = this;
        }

        public async Task Load() {
            Logger.LogInfo("Loading HttpEngine...");
            _sites.Clear();
            LoadDefaultDirectives();
            await LoadSites(_sitesPath);
            _webHost = new WebHostBuilder()
                .UseStartup<WebServer>()
                .UseKestrel(ConfigureKestrel)
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddProvider(new HtcLoggerProvider(Logger));
                })
                .ConfigureServices(configureServices => {
                    configureServices.AddSingleton(_sites);
                    configureServices.AddSingleton(_directiveManager);
                })
                .Build();
            Logger.LogInfo("Loaded HttpEngine");
        }

        public async Task Start() {
            Logger.LogInfo("Starting HttpEngine...");
            await _webHost.StartAsync();
            Logger.LogInfo("Started HttpEngine");
        }

        public async Task Stop() {
            Logger.LogInfo("Stopping HttpEngine...");
            await _webHost.StopAsync();
            Logger.LogInfo("Stopped HttpEngine");
        }

        public async Task LoadSites(string sitesPath) {
            _sites.Clear();
            foreach (string fileName in Directory.GetFiles(sitesPath, "*.json", SearchOption.TopDirectoryOnly)) {
                try {
                    await LoadSite(fileName);
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load site config file '{fileName}'.", ex);
                }
            }
        }

        private async Task LoadSite(string filename) {
            Logger.LogInfo($"Loading site '{Path.GetFileName(filename)}'...");
            await using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var jsonDocument = await JsonDocument.ParseAsync(fs);
            _sites.Add(new Site(SiteConfig.ParseConfig(jsonDocument.RootElement)));
        }

        private void LoadDefaultDirectives() {
            _directiveManager.RegisterDirective<TestDirective>("test");

            _directiveManager.RegisterDirective<MvcDirective>("try_mvc");
            _directiveManager.RegisterDirective<PagesDirective>("try_pages");
            _directiveManager.RegisterDirective<IndexDirective>("try_indexes");
            _directiveManager.RegisterDirective<StaticFileDirective>("try_files");
            _directiveManager.RegisterDirective<ExtensionProcessorDirective>("try_extensions");

            _directiveManager.RegisterDirective<AddHeaderDirective>("add_headers");
            _directiveManager.RegisterDirective<ListDirectoryDirective>("list_directory");
            _directiveManager.RegisterDirective<ReturnDirective>("return");
        }

        #region Static

        public static void RegisterPage(IPlugin plugin, string path, IHttpPage page) {
            if (_httpEngine == null) throw new HttpEngineNotInitializedException();
            foreach (var site in _httpEngine._sites) {
                if (!site.HasPermission(plugin)) continue;
                site.RegisterPage(path, page);
            }
        }

        public static void RegisterMvc(IPlugin plugin, HttpMvc mvc) {
            if (_httpEngine == null) throw new HttpEngineNotInitializedException();
            foreach (var site in _httpEngine._sites) {
                if (!site.HasPermission(plugin)) continue;
                site.RegisterMvc(mvc);
            }
        }

        public static void RegisterExtensionProcessor(IPlugin plugin, string extension, IExtensionProcessor extensionProcessor) {
            if (_httpEngine == null) throw new HttpEngineNotInitializedException();
            _httpEngine.Logger.LogInfo($"Registering extension processor for '{extension}'");
            foreach (var site in _httpEngine._sites) {
                if (!site.HasPermission(plugin)) continue;
                site.RegisterExtensionProcessor(extension, extensionProcessor);
            }
        }

        public static void RegisterIndex(IPlugin plugin, string fileName) {
            if (_httpEngine == null) throw new HttpEngineNotInitializedException();
            _httpEngine.Logger.LogInfo($"Registering index '{fileName}'");
            foreach (var site in _httpEngine._sites) {
                if (!site.HasPermission(plugin)) continue;
                site.RegisterIndexFilename(fileName);
            }
        }

        #endregion

        #region Configure Kestrel

        private void ConfigureKestrel(KestrelServerOptions options) {
            var ipEndPointList = new List<IPEndPointConfig>();
            foreach (var site in _sites) {
                foreach (string host in site.Config.Hosts) {
                    string[] hostData = host.Split(":", 2);
                    string ipAddress = hostData[0];
                    int port = int.Parse(hostData[1]);

                    if (TryGetIPEndPoint(ipEndPointList, ipAddress, port, out var endPoint)) {
                        foreach (var sslConfig in site.Config.SslConfigs) {
                            endPoint.AddCertificate(sslConfig.GetCertificate());
                        }
                    } else {
                        endPoint = new IPEndPointConfig(ipAddress, port);
                        foreach (var sslConfig in site.Config.SslConfigs) {
                            endPoint.AddCertificate(sslConfig.GetCertificate());
                        }

                        ipEndPointList.Add(endPoint);
                    }
                }
            }

            foreach (var endPoint in ipEndPointList) {
                options.Listen(endPoint.IPAddress, endPoint.Port, listenOptions => {
                    if (endPoint.Certificates.Count > 0) {
                        Logger.LogInfo($"Listening https on '{endPoint.IPAddress}:{endPoint.Port}'");
                        foreach (var certificate in endPoint.Certificates) {
                            listenOptions.UseHttps(certificate);
                        }
                    } else {
                        Logger.LogInfo($"Listening http on '{endPoint.IPAddress}:{endPoint.Port}'");
                    }

                    listenOptions.NoDelay = endPoint.NoDelay;
                });
            }
        }

        private static bool TryGetIPEndPoint(IEnumerable<IPEndPointConfig> list, string address, int port, out IPEndPointConfig config) {
            foreach (var ipEndPointConfig in list) {
                if (!ipEndPointConfig.Equals(address, port)) continue;
                config = ipEndPointConfig;
                return true;
            }

            config = null;
            return false;
        }

        private class IPEndPointConfig {
            public IPAddress IPAddress { get; }

            public int Port { get; }

            public bool NoDelay { get; }

            public List<X509Certificate2> Certificates { get; }

            public IPEndPointConfig(string ipAddress, int port) {
                IPAddress = IPAddress.Parse(ipAddress);
                Port = port;
                Certificates = new List<X509Certificate2>();
                NoDelay = true;
            }

            public void AddCertificate(X509Certificate2 certificate) {
                Certificates.Add(certificate);
            }

            public bool Equals(string address, int port) {
                return IPAddress.ToString().Equals(address) && Port == port;
            }
        }

        #endregion
    }
}