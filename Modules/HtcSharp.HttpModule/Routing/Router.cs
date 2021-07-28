using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Routing {
    public class Router {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly SiteCollection _sites;

        public Router(IServiceProvider serviceProvider) {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            _sites = serviceProvider.GetService(typeof(SiteCollection)) as SiteCollection;
            if (_sites == null) throw new NullReferenceException("Failed to get SiteCollection.");
            foreach (var site in _sites) {
                site.InitializeLocations(serviceProvider);
            }
        }

        public async Task ProcessRequest(HttpContext httpContext) {
#if DEBUG
            var stopWatch = new Stopwatch();
            stopWatch.Start();
#endif
            var matchedSite = _sites.FirstOrDefault(site => site.Match(httpContext));
#if DEBUG
            stopWatch.Stop();
            Logger.LogDebug($"Router took {stopWatch.ElapsedMilliseconds}ms to match location.");
#endif
            if (matchedSite == null) {
            } else {
#if DEBUG
                stopWatch.Reset();
                stopWatch.Start();
#endif
                var htcContext = new HtcHttpContext(httpContext, matchedSite);
                await matchedSite.ProcessRequest(htcContext);
#if DEBUG
                stopWatch.Stop();
                Logger.LogDebug($"Router took {stopWatch.ElapsedMilliseconds}ms to process request.");
#endif
            }
        }
    }
}