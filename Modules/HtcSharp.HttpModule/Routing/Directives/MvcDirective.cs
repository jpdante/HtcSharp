﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class MvcDirective : IDirective {

        private readonly DirectiveDelegate _next;

        private readonly List<string> _paths;

        public MvcDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            if (config.TryGetProperty("paths", out var filesProperty)) {
                switch (filesProperty.ValueKind) {
                    case JsonValueKind.String: {
                        string value = filesProperty.GetString();
                        if (string.IsNullOrEmpty(value)) throw new NullReferenceException("'paths' property cannot be null or empty.");
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
                    default:
                        throw new NullReferenceException("'paths' property unknown type.");
                }
            } else {
                _paths = new List<string> {
                    "$uri"
                };
            }
        }

        public Task Invoke(HtcHttpContext httpContext) {
            foreach (string replacePath in _paths) {
                string path = replacePath.Replace("$uri", httpContext.Request.Path.Value);
                foreach (var mvc in httpContext.Site.Mvcs) {
                    if (mvc.Match(httpContext, path)) {
                        return mvc.OnHttpRequest(httpContext, path);
                    }
                }
                return _next(httpContext);
            }
            return _next(httpContext);
        }
    }
}