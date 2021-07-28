using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Routing {
    public class Site : IReadOnlySite {

        private readonly HashSet<string> _domains;

        private readonly bool _matchAny;

        private Collection<SiteLocation> _locations;

        public SiteConfig Config { get; }

        public IFileProvider FileProvider { get; }

        public Site(SiteConfig siteConfig) {
            Config = siteConfig;
            _domains = new HashSet<string>();
            _matchAny = false;
            foreach (string domain in Config.Domains) {
                if (domain.Equals("*")) _matchAny = true;
                _domains.Add(domain);
            }
            _locations = new Collection<SiteLocation>();
            if (string.IsNullOrEmpty(Config.RootDirectory)) throw new NullReferenceException("Root path was not specified.");
            FileProvider = new PhysicalFileProvider(Path.GetFullPath(Config.RootDirectory));
        }

        public void InitializeLocations(IServiceProvider serviceProvider) {
            _locations = new Collection<SiteLocation>();
            foreach (var locationConfig in Config.Locations) {
                _locations.Add(new SiteLocation(locationConfig, serviceProvider));
            }
        }

        public bool Match(HttpContext httpContext) {
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