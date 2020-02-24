using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Core.Engine.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Default;
using HtcSharp.HttpModule.Logging;
using HtcSharp.HttpModule.Net.Socket;
using HtcSharp.HttpModule.Options;
using HtcSharp.HttpModule.Routing;
using HtcSharp.HttpModule.Routing.Error;
using HtcSharp.HttpModule.Routing.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using LogLevel = HtcSharp.Core.Logging.Abstractions.LogLevel;

namespace HtcSharp.HttpModule {
    public class HttpEngine : IEngine {

        public string Name => "HtcHttp";

        private Core.Logging.Abstractions.ILogger _logger;
        private IServiceProvider _serviceProvider;
        private ILoggerFactory _loggerFactory;
        private SocketTransportFactory _socketTransportFactory;
        private KestrelServer _kestrelServer;
        private CancellationToken _cancellationToken;

        internal List<HttpServerConfig> ServerConfigs;
        internal Dictionary<string, HttpServerConfig> DomainDictionary;

        public Task Load(JObject config, Core.Logging.Abstractions.ILogger logger) {
            _logger = logger;
            Configure(config);
            UrlMapper.RegisterIndexFile("index.html");
            UrlMapper.RegisterIndexFile("index.htm");
            _serviceProvider = SetupServiceCollection().BuildServiceProvider();
            _loggerFactory = LoggerFactory.Create(builder => {
                builder.AddProvider(new HtcLoggerProvider(_logger));
            });
            _socketTransportFactory = new SocketTransportFactory(GetSocketTransportOptions(config), _loggerFactory);
            _kestrelServer = new KestrelServer(GetKestrelServerOptions(config, _serviceProvider), _socketTransportFactory, _loggerFactory);
            return Task.CompletedTask;
        }

        public async Task Start() {
            _cancellationToken = new CancellationToken();
            var logger = _loggerFactory.CreateLogger("HtcLogger");
            await _kestrelServer.StartAsync(new HostingApplication(this, logger, _serviceProvider.GetRequiredService<IHttpContextFactory>()), _cancellationToken);
        }

        public async Task Stop() {
            await _kestrelServer.StopAsync(_cancellationToken);
        }

        private ServiceCollection SetupServiceCollection() {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IHttpContextFactory, DefaultHttpContextFactory>();
            serviceCollection.AddTransient<ILogger, Logger<KestrelServer>>();
            //var listener = new DiagnosticListener("HtcSharpServer");
            //serviceCollection.AddSingleton<DiagnosticListener>(listener);
            //serviceCollection.AddSingleton<DiagnosticSource>(listener);
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            return serviceCollection;
        }

        private void Configure(JObject config) {
            ServerConfigs = new List<HttpServerConfig>();
            DomainDictionary = new Dictionary<string, HttpServerConfig>();
            var servers = config.GetValue("Servers", StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if (servers == null) return;
            foreach (var jToken in servers) {
                var server = (JObject)jToken;
                List<string> hosts = GetValues<string>(server, "Hosts");
                List<string> domains = GetValues<string>(server, "Domains");
                string root = HtcIOUtils.ReplacePathTags(GetValue<string>(server, "Root"));
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                var useSsl = GetValue<bool>(server, "SSL");
                string certificate = null;
                string password = null;
                if (useSsl) {
                    certificate = HtcIOUtils.ReplacePathTags(GetValue<string>(server, "Certificate"));
                    password = GetValue<string>(server, "Password");
                }
                var locationManager = ContainsKey(server, "Locations") ? new HttpLocationManager(GetValue<JToken>(server, "Default"), GetValue<JObject>(server, "Locations")) : new HttpLocationManager(GetValue<JToken>(server, "Default"), null);
                var errorMessageManager = new ErrorMessageManager();
                if (ContainsKey(server, "ErrorPages")) {
                    foreach ((string key, var value) in GetValue<JObject>(server, "ErrorPages")) {
                        if (int.TryParse(key, out int pageStatusCode)) errorMessageManager.RegisterOverridePage(new FilePageMessage(value.Value<string>(), pageStatusCode));
                    }
                }
                var serverConfig = new HttpServerConfig(hosts, domains, root, useSsl, certificate, password, locationManager, errorMessageManager);
                ServerConfigs.Add(serverConfig);
                if (domains.Contains("*")) {
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    //foreach (string host in hosts) {
                        string key = useSsl ? $"1*" : $"0*";
                        if (DomainDictionary.ContainsKey(key)) continue;
                        DomainDictionary.Add(key, serverConfig);
                    //}
                } else {
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (string domain in domains) {
                        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                        //foreach (string host in hosts) {
                            string key = useSsl ? $"1{domain}" : $"0{domain}";
                            if (DomainDictionary.ContainsKey(key)) continue;
                            DomainDictionary.Add(key, serverConfig);
                        //}
                    }
                }
            }
        }

        private static IOptions<SocketTransportOptions> GetSocketTransportOptions(JObject config) {
            var socketTransportOptions = new SocketTransportOptions();
            IOptions<SocketTransportOptions> optionFactory = Microsoft.Extensions.Options.Options.Create(socketTransportOptions);
            return optionFactory;
        }

        private IOptions<KestrelServerOptions> GetKestrelServerOptions(JObject config, IServiceProvider serviceProvider) {
            var endpoints = new List<string>();
            var kestrelServerOptions = new KestrelServerOptions { ApplicationServices = serviceProvider };
            foreach (var serverConfig in ServerConfigs) {
                if (serverConfig.UseSsl) {
                    foreach (string endpoint in serverConfig.Endpoints) {
                        if (endpoints.Contains(endpoint)) continue;
                        endpoints.Add(endpoint);
                        kestrelServerOptions.Listen(IPEndPoint.Parse(endpoint), options => {
                            options.UseHttps(serverConfig.Certificate, serverConfig.CertificatePassword);
                        });
                    }
                } else {
                    foreach (string endpoint in serverConfig.Endpoints) {
                        if (endpoints.Contains(endpoint)) continue;
                        endpoints.Add(endpoint);
                        kestrelServerOptions.Listen(IPEndPoint.Parse(endpoint));
                    }
                }
            }
            //kestrelServerOptions.Configure();
            IOptions<KestrelServerOptions> optionFactory = Microsoft.Extensions.Options.Options.Create(kestrelServerOptions);
            return optionFactory;
        }

        private static List<T> GetValues<T>(JObject jObject, string key) {
            var jArray = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if (jArray == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            return jArray.Select(obj => obj.Value<T>()).ToList();
        }

        private static T GetValue<T>(JObject jObject, string key) {
            var jToken = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase);
            if (jToken == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            return jToken.Value<T>();
        }

        private static bool ContainsKey(JObject jObject, string key) {
            var jToken = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase);
            return jToken != null;
        }
    }
}