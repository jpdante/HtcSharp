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
    public class StaticFileDirective : IDirective {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly DirectiveDelegate _next;

        private readonly List<string> _files;

        public StaticFileDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            if (config.TryGetProperty("files", out var filesProperty)) {
                switch (filesProperty.ValueKind) {
                    case JsonValueKind.String: {
                        string value = filesProperty.GetString();
                        if (string.IsNullOrEmpty(value)) throw new NullReferenceException("'files' property cannot be null or empty.");
                        _files = new List<string> {value};
                        break;
                    }
                    case JsonValueKind.Array: {
                        _files = new List<string>();
                        foreach (var arrayElement in filesProperty.EnumerateArray()) {
                            string value = arrayElement.GetString();
                            if (string.IsNullOrEmpty(value)) continue;
                            _files.Add(value);
                        }

                        break;
                    }
                }
            } else {
                _files = new List<string> {
                    "$uri"
                };
            }
        }

        public Task Invoke(HtcHttpContext httpContext) {
            foreach (string replaceName in _files) {
                PathString fileName = replaceName.Replace("$uri", httpContext.Request.Path.Value);
                var staticFileContext = new StaticFileContext(httpContext, httpContext.Site.FileProvider, fileName);
                if (!staticFileContext.LookupFileInfo()) continue;
                return staticFileContext.ServeStaticFile(httpContext, _next);
            }
            return _next(httpContext);
        }
    }
}