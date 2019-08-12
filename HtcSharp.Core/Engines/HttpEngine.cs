using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.Core;
using HtcSharp.Core.Components.Http;
using HtcSharp.Core.IO;
using HtcSharp.Core.Managers;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Models.Http.Pages;
using HtcSharp.Core.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Engines {
    public class HttpEngine : Engine {

        private IWebHost _webHostServer;

        public override void Start() {
            ServersInfo = new List<HttpServerInfo>();
            DomainServers = new Dictionary<string, HttpServerInfo>();
            if (EngineConfig == null) throw new NullReferenceException("Engine config is null!");
            CreateServers();
            _webHostServer = CreateWebHost(new string[] { }).Build();
            _webHostServer.Start();
        }

        public override void Stop() {
            _webHostServer.StopAsync().GetAwaiter().GetResult();
        }

        private IWebHostBuilder CreateWebHost(string[] args) {
            var endpoints = new List<string>();
            return WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                config.AddJsonFile(HtcServer.Context.AspNetConfigPath);
            })
            .UseKestrel(options => {
                foreach(var serverInfo in ServersInfo) {
                    if(serverInfo.UseSsl) {
                        foreach (var endPoint in serverInfo.Hosts) {
                            if (endpoints.Contains($"{endPoint.Address}{endPoint.Port}")) continue;
                            endpoints.Add($"{endPoint.Address}{endPoint.Port}");
                            options.Listen(endPoint.Address, endPoint.Port, listenOptions => {
                                listenOptions.UseHttps(serverInfo.Certificate, serverInfo.Password);
                            });
                        }
                    } else {
                        foreach(var endPoint in serverInfo.Hosts) {
                            if (endpoints.Contains($"{endPoint.Address}{endPoint.Port}")) continue;
                            endpoints.Add($"{endPoint.Address}{endPoint.Port}");
                            options.Listen(endPoint.Address, endPoint.Port);
                        }
                    }
                }
            })
            .ConfigureServices(configureServices => {
                configureServices.AddSingleton<HttpEngine>(this);
            })
            .UseStartup<HttpStartup>(); 
        }

        private void CreateServers() {
            var servers = EngineConfig.GetValue("Servers", StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if (servers == null) return;
            foreach (var jToken in servers) {
                var server = (JObject) jToken;
                var hosts = GetValues<string>(server, "Hosts");
                var domains = GetValues<string>(server, "Domains");
                var root = IoUtils.ReplacePathTags(GetValue<string>(server, "Root"));
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                var useSsl = GetValue<bool>(server, "SSL");
                string certificate = null;
                string password = null;
                if (useSsl) {
                    certificate = IoUtils.ReplacePathTags(GetValue<string>(server, "Certificate"));
                    password = GetValue<string>(server, "Password");
                }
                var locationManager = ContainsKey(server, "Locations") ? new HttpLocationManager(GetValue<JToken>(server, "Default"), GetValue<JObject>(server, "Locations")) : new HttpLocationManager(GetValue<JToken>(server, "Default"), null);
                var errorMessagesManager = new ErrorMessagesManager();
                if (ContainsKey(server, "ErrorPages")) {
                    foreach(var (key, value) in GetValue<JObject>(server, "ErrorPages")) {
                        if (int.TryParse(key, out var pageStatusCode)) errorMessagesManager.RegisterOverridePage(new FilePageMessage(value.Value<string>(), pageStatusCode));
                    }
                }
                var serverInfo = new HttpServerInfo(hosts.AsReadOnly(), domains, root, useSsl, certificate, password, locationManager, errorMessagesManager);
                ServersInfo.Add(serverInfo);
                foreach (var domain in domains) {
                    // ! ISSUE ! Need future fix, for decoding the domain between ports
                    if (DomainServers.ContainsKey(domain)) continue;
                    DomainServers.Add(domain, serverInfo);
                }
            }
        }

        private static List<T> GetValues<T>(JObject jObject, string key) {
            var result = new List<T>();
            var jArray = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if (jArray == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            foreach (var obj in jArray) {
                result.Add(obj.Value<T>());
            }
            return result;
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

        public List<HttpServerInfo> ServersInfo { get; private set; }
        public Dictionary<string, HttpServerInfo> DomainServers { get; private set; }
    }
}
