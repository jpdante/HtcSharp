using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Routing;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ILogger = HtcSharp.Logging.ILogger;

namespace HtcSharp.HttpModule.Core {
    internal class WebServer {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private Router _router;

        public void ConfigureServices(IServiceCollection services) {
        }

        public void Configure(IApplicationBuilder app) {
            _router = new Router(app.ApplicationServices);
            app.Run(OnRequest);
        }

        public async Task OnRequest(HttpContext context) {
            try {
                await _router.ProcessRequest(context);
            } catch (Exception ex) {
                Logger.LogError(message: null, ex);
            }
        }
    }
}