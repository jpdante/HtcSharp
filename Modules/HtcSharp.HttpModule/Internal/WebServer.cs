using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Middleware;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ILogger = HtcSharp.Logging.ILogger;

namespace HtcSharp.HttpModule.Internal {
    internal class WebServer {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private MiddlewareContext _middlewareContext;

        public void ConfigureServices(IServiceCollection services) {
            
        }

        public void Configure(IApplicationBuilder app) {
            _middlewareContext = app.ApplicationServices.GetService(typeof(MiddlewareContext)) as MiddlewareContext;
            app.Run(OnRequest);
        }

        public async Task OnRequest(HttpContext context) {
            try {
                var htcContext = new HtcHttpContext(context);
                await _middlewareContext.Invoke(htcContext);
            } catch (Exception ex) {
                Logger.LogError(null, ex);
            }
        }

    }
}