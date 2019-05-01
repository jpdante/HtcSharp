using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.Core;
using HtcSharp.Core.Components.Http;
using HtcSharp.Core.IO;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Engines {
    public class HttpEngine : Engine {

        private IWebHost _webHostServer;
        private List<HttpServerInfo> _serversInfo;
        private Dictionary<string, HttpServerInfo> _domainServers;

        public override void Start() {
            _serversInfo = new List<HttpServerInfo>();
            _domainServers = new Dictionary<string, HttpServerInfo>();
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
                foreach(var serverInfo in _serversInfo) {
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
                var domain = GetValue<string>(server, "Domain");
                var root = IoUtils.ReplacePathTags(GetValue<string>(server, "Root"));
                var useSsl = GetValue<bool>(server, "SSL");
                string certificate = null;
                string password = null;
                if (useSsl) {
                    certificate = IoUtils.ReplacePathTags(GetValue<string>(server, "Certificate"));
                    password = GetValue<string>(server, "Password");
                }
                HttpReWriter reWriter = null;
                if (ContainsKey(server, "Rewrites")) {
                    reWriter = new HttpReWriter(GetValue<JObject>(server, "Rewrites"));
                }
                var serverInfo = new HttpServerInfo(hosts.AsReadOnly(), domain, root, useSsl, certificate, password, reWriter);
                _serversInfo.Add(serverInfo);
                _domainServers.Add(domain, serverInfo);
            }
        }

        private List<T> GetValues<T>(JObject jObject, string key) {
            var result = new List<T>();
            var jArray = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if (jArray == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            foreach (var obj in jArray) {
                result.Add(obj.Value<T>());
            }
            return result;
        }

        private T GetValue<T>(JObject jObject, string key) {
            var jToken = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase);
            if (jToken == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            return jToken.Value<T>();
        }

        private bool ContainsKey(JObject jObject, string key) {
            var jToken = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase);
            return jToken != null;
        }

        public List<HttpServerInfo> getServersInfo => _serversInfo;
        public Dictionary<string, HttpServerInfo> getDomainServers => _domainServers;
    }
}
