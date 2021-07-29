using System.Collections.Generic;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IReadOnlySite {

        public IFileProvider FileProvider { get; }

        internal Dictionary<string, IHttpPage> Pages { get; }

        internal Dictionary<string, HttpMvc> MvcPages { get; }

        internal Dictionary<string, IExtensionProcessor> FileExtensions { get; }

        public bool Match(HttpContext httpContext);

        public bool HasPermission(IPlugin plugin);

    }
}