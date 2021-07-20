using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Internal;
using HtcSharp.HttpModule.Logging;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ILogger = HtcSharp.Logging.ILogger;

namespace HtcSharp.HttpModule {
    public class HttpEngine {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private IWebHost _webHost;

        private List<Site> _sites;

        public HttpEngine() {
            _sites = new List<Site>();
        }

        public Task Load() {
            Logger.LogInfo("Loading HttpEngine...");
            _sites.Clear();
            _webHost = new WebHostBuilder()
            .UseStartup<WebServer>()
            .UseKestrel(options => {
                options.ListenAnyIP(80, listenOptions => {
                    listenOptions.NoDelay = true;
                });
            })
            .ConfigureLogging(logging => {
                logging.ClearProviders();
                logging.AddProvider(new HtcLoggerProvider(Logger));
            })
            .Build();
            Logger.LogInfo("Loaded HttpEngine");
            return Task.CompletedTask;
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

        public async Task LoadSites() {

        }

        public async Task LoadSite(string filename) {
            await using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var jsonDocument = await JsonDocument.ParseAsync(fs);
            _sites.Add(new Site(SiteConfig.ParseConfig(jsonDocument.RootElement)));
        }
    }
}