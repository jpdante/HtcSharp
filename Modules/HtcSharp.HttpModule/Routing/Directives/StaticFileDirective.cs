﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.StaticFiles;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class StaticFileDirective : IDirective {

        private readonly DirectiveDelegate _next;

        private readonly List<string> _paths;

        public StaticFileDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            if (config.TryGetProperty("paths", out var filesProperty)) {
                switch (filesProperty.ValueKind) {
                    case JsonValueKind.String: {
                        string value = filesProperty.GetString();
                        if (string.IsNullOrEmpty(value)) throw new NullReferenceException("'files' property cannot be null or empty.");
                        _paths = new List<string> {value};
                        break;
                    }
                    case JsonValueKind.Array: {
                        _paths = new List<string>();
                        foreach (var arrayElement in filesProperty.EnumerateArray()) {
                            string value = arrayElement.GetString();
                            if (string.IsNullOrEmpty(value)) continue;
                            _paths.Add(value);
                        }

                        break;
                    }
                }
            } else {
                _paths = new List<string> {
                    "$uri"
                };
            }
        }

        public Task Invoke(HtcHttpContext httpContext) {
            foreach (string replaceName in _paths) {
                PathString fileName = replaceName.Replace("$uri", httpContext.Request.Path.Value);
                var staticFileContext = new StaticFileContext(httpContext, httpContext.Site.FileProvider, fileName);
                if (!staticFileContext.LookupFileInfo()) continue;
                return staticFileContext.ServeStaticFile(httpContext, _next);
            }
            return _next(httpContext);
        }
    }
}