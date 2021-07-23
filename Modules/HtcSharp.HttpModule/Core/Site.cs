using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Middleware;

namespace HtcSharp.HttpModule.Core {
    public class Site {

        private readonly RequestDelegate _requestDelegate;

        private readonly HashSet<string> _domains;

        private readonly bool _matchAny;

        public SiteConfig Config { get; }

        public Site(SiteConfig siteConfig) {
            Config = siteConfig;
            _domains = new HashSet<string>();
            _matchAny = false;
        }

        public void LoadConfig() {
            
        }

        public bool Match(HtcHttpContext httpContext) {
            if (_matchAny) return true;
            return !httpContext.Request.Host.HasValue || _domains.Contains(httpContext.Request.Host.Value);
        }

        public async Task ProcessRequest(HtcHttpContext httpContext) {
            await _requestDelegate(httpContext);
        }

    }
}