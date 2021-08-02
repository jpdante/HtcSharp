using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Core.Exceptions;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Core {
    public class Site : IReadOnlySite {

        private readonly bool _matchAny;
        private readonly HashSet<string> _domains;
        private readonly List<string> _indexes;
        private readonly List<HttpMvc> _mvcs;
        private readonly Dictionary<string, IHttpPage> _pages;
        private readonly Dictionary<string, IExtensionProcessor> _fileExtensions;

        private Collection<SiteLocation> _locations;

        IReadOnlyList<string> IReadOnlySite.Indexes => _indexes;
        IReadOnlyList<HttpMvc> IReadOnlySite.Mvcs => _mvcs;
        IReadOnlyDictionary<string, IHttpPage> IReadOnlySite.Pages => _pages;
        IReadOnlyDictionary<string, IExtensionProcessor> IReadOnlySite.FileExtensions => _fileExtensions;

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
            _indexes = new List<string>();
            _pages = new Dictionary<string, IHttpPage>();
            _mvcs = new List<HttpMvc>();
            _fileExtensions = new Dictionary<string, IExtensionProcessor>();
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

        public bool HasPermission(IPlugin plugin) {
            if (!Config.PluginsEnabled) return false;
            if (Config.ForbiddenPlugins.Contains(plugin.Name)) return false;
            return Config.AllowedPlugins.Contains(plugin.Name) || Config.DefaultPluginPermission;
        }

        public async Task ProcessRequest(HtcHttpContext httpContext) {
            foreach (var location in _locations) {
                if (!location.Match(httpContext)) continue;
                await location.ProcessRequest(httpContext);
                break;
            }
        }

        internal void RegisterPage(string path, IHttpPage page) {
            if (_pages.ContainsKey(path)) throw new PageAlreadyExistsException(path);
            _pages.Add(path, page);
        }

        internal void RegisterMvc(HttpMvc httpMvc) {
            if (_mvcs.Contains(httpMvc)) throw new MvcAlreadyExistsException(httpMvc.GetType().FullName);
            _mvcs.Add(httpMvc);
        }

        internal void RegisterExtensionProcessor(string extension, IExtensionProcessor extensionProcessor) {
            if (_fileExtensions.ContainsKey(extension)) throw new PageAlreadyExistsException(extension);
            _fileExtensions.Add(extension, extensionProcessor);
        }

        internal void RegisterIndexFilename(string fileName) {
            if (!fileName.StartsWith("/")) fileName = $"/{fileName}";
            if (_indexes.Contains(fileName)) throw new MvcAlreadyExistsException(fileName);
            _indexes.Add(fileName);
        }
    }
}