using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.StaticFiles;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class IndexDirective : IDirective {

        private readonly DirectiveDelegate _next;

        private readonly List<string> _files;

        public IndexDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            if (config.TryGetProperty("files", out var filesProperty)) {
                switch (filesProperty.ValueKind) {
                    case JsonValueKind.String: {
                        string value = filesProperty.GetString();
                        if (string.IsNullOrEmpty(value)) throw new NullReferenceException("'files' property cannot be null or empty.");
                        _files = new List<string> { value.StartsWith("/") ? value : $"/{value}" };
                        break;
                    }
                    case JsonValueKind.Array: {
                        _files = new List<string>();
                        foreach (var arrayElement in filesProperty.EnumerateArray()) {
                            string value = arrayElement.GetString();
                            if (string.IsNullOrEmpty(value)) continue;
                            _files.Add(value.StartsWith("/") ? value : $"/{value}");
                        }

                        break;
                    }
                }
            } else {
                _files = new List<string> {
                    "/index.html",
                    "/index.htm"
                };
            }
        }

        public Task Invoke(HtcHttpContext httpContext) {
            foreach (string fileName in _files) {
                var path = httpContext.Request.Path.Add(fileName);
                var staticFileContext = new StaticFileContext(httpContext, httpContext.Site.FileProvider, path);
                if (!staticFileContext.LookupFileInfo()) continue;
                return staticFileContext.ServeStaticFile(httpContext, _next);
            }
            return _next(httpContext);
        }
    }
}