using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        public HttpEngine() {

        }

        public Task Load() {
            Logger.LogInfo("Loading HttpEngine...");
            _webHost = new WebHostBuilder()
            .UseStartup<WebServer>()
            .UseKestrel(options => {
                options.ListenAnyIP(80, listenOptions => {
                    listenOptions.NoDelay = true;
                    listenOptions.UseHttps()
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
    }
}