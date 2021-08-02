using System.Collections.Generic;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IReadOnlySite {

        public IFileProvider FileProvider { get; }

        public TemplateManager TemplateManager { get; }

        internal IReadOnlyList<string> Indexes { get; }

        internal IReadOnlyList<HttpMvc> Mvcs { get; }

        internal IReadOnlyDictionary<string, IHttpPage> Pages { get; }

        internal IReadOnlyDictionary<string, IExtensionProcessor> FileExtensions { get; }

        public bool Match(HttpContext httpContext);

        public bool HasPermission(IPlugin plugin);

    }
}