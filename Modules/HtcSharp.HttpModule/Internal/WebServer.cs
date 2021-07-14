using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ILogger = HtcSharp.Logging.ILogger;

namespace HtcSharp.HttpModule.Internal {
    internal class WebServer {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public void ConfigureServices(IServiceCollection services) {

        }

        public void Configure(IApplicationBuilder app) {
            app.Run(OnRequest);
        }

        public async Task OnRequest(HttpContext context) {
            await context.Response.WriteAsync("Test");
        }

    }
}