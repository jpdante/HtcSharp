using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Hosting;

namespace HtcSharp.HttpModule {
    public class HttpEngine {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private IWebHost _webHost;

        public HttpEngine() {

        }

        public Task Load() {
            Logger.LogInfo("Loading HttpEngine...");
            var webHostBuilder = new WebHostBuilder();
            _webHost = webHostBuilder.UseKestrel(options => {
                options.ListenAnyIP(80, listenOptions => {
                    listenOptions.NoDelay = true;
                });
            }).Build();
            return Task.CompletedTask;
        }

        public async Task Start() {
            Logger.LogInfo("Starting HttpEngine...");
            await _webHost.StartAsync();
        }

        public async Task Stop() {
            Logger.LogInfo("Stopping HttpEngine...");
            await _webHost.StopAsync();
        }
    }
}