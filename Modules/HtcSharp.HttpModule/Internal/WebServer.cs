using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ILogger = HtcSharp.Logging.ILogger;

namespace HtcSharp.HttpModule.Internal {
    internal class WebServer {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private SiteCollection _sites;

        public void ConfigureServices(IServiceCollection services) {
        }

        public void Configure(IApplicationBuilder app) {
            _sites = app.ApplicationServices.GetService(typeof(SiteCollection)) as SiteCollection;
            app.Run(OnRequest);
        }

        public async Task OnRequest(HttpContext context) {
            try {
                var htcContext = new HtcHttpContext(context);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var matchedSite = _sites.FirstOrDefault(site => site.Match(htcContext));
                if (matchedSite == null) {

                } else {
                    await matchedSite.ProcessRequest(htcContext);
                }
                stopWatch.Stop();
                Logger.LogDebug($"Middlwares took {stopWatch.ElapsedMilliseconds}ms to complete.");
            } catch (Exception ex) {
                Logger.LogError(null, ex);
            }
        }
    }
}