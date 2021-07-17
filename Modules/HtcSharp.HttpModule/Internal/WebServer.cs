using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Middleware;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ILogger = HtcSharp.Logging.ILogger;
using RequestDelegate = HtcSharp.HttpModule.Middleware.RequestDelegate;

namespace HtcSharp.HttpModule.Internal {
    internal class WebServer {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private RequestDelegate _middlewareRequestDelegate;

        public void ConfigureServices(IServiceCollection services) {
        }

        public void Configure(IApplicationBuilder app) {
            _middlewareRequestDelegate = new MiddlewareBuilder(app.ApplicationServices)
                .UseRewriter()
                .UseRouter()
                .UseMvc()
                .UsePages()
                .UseHttpEvents()
                .UseStaticFiles()
                .Build();
            app.Run(OnRequest);
        }

        public async Task OnRequest(HttpContext context) {
            try {
                var htcContext = new HtcHttpContext(context);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                await _middlewareRequestDelegate(htcContext);
                stopWatch.Stop();
                Logger.LogDebug($"Middlwares took {stopWatch.ElapsedMilliseconds}ms to complete.");
            } catch (Exception ex) {
                Logger.LogError(null, ex);
            }
        }
    }
}