using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTCSharp.Core.Components.Http;
using HTCSharp.Core.IO;
using HTCSharp.Core.Models.Http;
using HTCSharp.Core.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace HTCSharp.Core.Engines {
    public class HttpEngine : Engine {

        private IWebHost webHostServer;
        private List<HttpServerInfo> serversInfo;
        private Dictionary<string, HttpServerInfo> domainServers;

        public override void Start() {
            serversInfo = new List<HttpServerInfo>();
            domainServers = new Dictionary<string, HttpServerInfo>();
            if (EngineConfig == null) throw new NullReferenceException("Engine config is null!");
            CreateServers();
            webHostServer = CreateWebHost(new string[] { }).Build();
            webHostServer.Start();
        }

        public override void Stop() {
            webHostServer.StopAsync().GetAwaiter().GetResult();
        }

        private IWebHostBuilder CreateWebHost(string[] args) {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            List<string> endpoints = new List<string>();
            builder.ConfigureAppConfiguration((hostingContext, config) => {
                config.AddJsonFile(HTCServer.Context.AspNetConfigPath);
            });
            builder.UseKestrel(options => {
                foreach(HttpServerInfo serverInfo in serversInfo) {
                    if(serverInfo.UseSSL) {
                        foreach (System.Net.IPEndPoint endPoint in serverInfo.Hosts) {
                            if (endpoints.Contains($"{endPoint.Address}{endPoint.Port}")) continue;
                            endpoints.Add($"{endPoint.Address}{endPoint.Port}");
                            options.Listen(endPoint.Address, endPoint.Port, listenOptions => {
                                listenOptions.UseHttps(serverInfo.Certificate, serverInfo.Password);
                            });
                        }
                    } else {
                        foreach(System.Net.IPEndPoint endPoint in serverInfo.Hosts) {
                            if (endpoints.Contains($"{endPoint.Address}{endPoint.Port}")) continue;
                            endpoints.Add($"{endPoint.Address}{endPoint.Port}");
                            options.Listen(endPoint.Address, endPoint.Port);
                        }
                    }
                }
            });
            builder.ConfigureServices(servicesColletion => {
                servicesColletion.AddSingleton<HttpEngine>(this);
            });
            builder.UseStartup<HttpStartup>();
            return builder;
        }

        private void CreateServers() {
            JArray servers = EngineConfig.GetValue("Servers", StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            foreach (JObject server in servers) {
                List<string> hosts = GetValues<string>(server, "Hosts");
                string domain = GetValue<string>(server, "Domain");
                string root = IOUtils.ReplacePathTags(GetValue<string>(server, "Root"));
                bool useSSL = GetValue<bool>(server, "SSL");
                string certificate = null;
                string password = null;
                if (useSSL) {
                    certificate = IOUtils.ReplacePathTags(GetValue<string>(server, "Certificate"));
                    password = GetValue<string>(server, "Password");
                }
                HttpRewriter rewriter = null;
                if (ContainsKey(server, "Rewrites")) {
                    rewriter = new HttpRewriter(GetValue<JObject>(server, "Rewrites"));
                }
                HttpServerInfo serverInfo = new HttpServerInfo(hosts.AsReadOnly(), domain, root, useSSL, certificate, password, rewriter);
                serversInfo.Add(serverInfo);
                domainServers.Add(domain, serverInfo);
            }
        }

        private List<T> GetValues<T>(JObject jObject, string key) {
            List<T> result = new List<T>();
            JArray jArray = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if (jArray == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            foreach (var obj in jArray) {
                result.Add(obj.Value<T>());
            }
            return result;
        }

        private T GetValue<T>(JObject jObject, string key) {
            JToken jToken = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase);
            if (jToken == null) throw new ArgumentNullException($"The '{key}' tag is null.");
            return jToken.Value<T>();
        }

        private bool ContainsKey(JObject jObject, string key) {
            JToken jToken = jObject.GetValue(key, StringComparison.CurrentCultureIgnoreCase);
            return jToken != null;
        }

        public List<HttpServerInfo> getServersInfo { get { return serversInfo; } }
        public Dictionary<string, HttpServerInfo> getDomainServers { get { return domainServers; } }
    }
}
