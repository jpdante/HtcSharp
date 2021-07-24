using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Middleware;

namespace HtcSharp.HttpModule.Core {
    public class Site {

        private readonly HashSet<string> _domains;

        private readonly bool _matchAny;

        private readonly Collection<SiteLocation> _locations;

        public SiteConfig Config { get; }

        public Site(SiteConfig siteConfig) {
            Config = siteConfig;
            _domains = new HashSet<string>();
            _matchAny = false;
            foreach (string domain in siteConfig.Domains) {
                if (domain.Equals("*")) _matchAny = true;
                _domains.Add(domain);
            }
            _locations = new Collection<SiteLocation>();
            foreach (var locationConfig in siteConfig.Locations) {
                _locations.Add(new SiteLocation(locationConfig));
            }
        }

        public bool Match(HtcHttpContext httpContext) {
            if (_matchAny) return true;
            return !httpContext.Request.Host.HasValue || _domains.Contains(httpContext.Request.Host.Value);
        }

        public async Task ProcessRequest(HtcHttpContext httpContext) {
            foreach (var location in _locations) {
                if (!location.Match(httpContext)) continue;
                await location.ProcessRequest(httpContext);
                break;
            }
        }

    }
}