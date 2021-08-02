using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class ReturnDirective : IDirective {

        private readonly DirectiveDelegate _next;

        private readonly int _type = 0;
        private readonly int _statusCode;
        private readonly string _data;

        public ReturnDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            if (config.TryGetProperty("value", out var filesProperty)) {
                if (filesProperty.ValueKind == JsonValueKind.String) {
                    string value = filesProperty.GetString();
                    if (string.IsNullOrEmpty(value)) throw new NullReferenceException("'value' property is null or empty.");
                    string[] returnData = value.Split(" ");
                    switch (returnData.Length) {
                        case 1 when int.TryParse(returnData[0], out int statusCode):
                            _statusCode = statusCode;
                            _type = 1;
                            break;
                        case 1:
                            _statusCode = 301;
                            _data = returnData[0];
                            _type = 2;
                            break;
                        case >= 2: {
                            if (!int.TryParse(returnData[0], out int statusCode)) return;
                            _statusCode = statusCode;
                            if (returnData[1][0].Equals('"') && returnData[^1][returnData[^1].Length - 1].Equals('"')) {
                                _type = 3;
                                for (var i = 2; i < returnData.Length; i++) {
                                    _data = i == returnData.Length - 1 ? $"{returnData[i]}" : $"{returnData[i]} ";
                                }
                            } else {
                                _type = 2;
                                for (var i = 2; i < returnData.Length; i++) {
                                    _data = i == returnData.Length - 1 ? $"{returnData[i]}" : $"{returnData[i]} ";
                                }
                            }
                            break;
                        }
                    }
                } else {
                    throw new NullReferenceException("'value' property unknown type.");
                }
            } else {
                throw new NullReferenceException("Missing property 'value' on return directive.");
            }
        }

        public Task Invoke(HtcHttpContext httpContext) {
            switch (_type) {
                case 1:
                    if (httpContext.Site.TemplateManager.TryGetTemplate(_statusCode.ToString(), out var template)) {
                        return template.SendTemplate(httpContext);
                    }
                    httpContext.Response.StatusCode = _statusCode;
                    break;
                case 2:
                    httpContext.Response.StatusCode = _statusCode;
                    httpContext.Response.Headers.Add("Location", ReplaceVars(httpContext, _data));
                    break;
                case 3:
                    httpContext.Response.StatusCode = _statusCode;
                    return httpContext.Response.WriteAsync(ReplaceVars(httpContext, _data));
            }
            return _next(httpContext);
        }

        private static string ReplaceVars(HttpContext httpContext, string data) {
            var dataBuilder = new StringBuilder(data);
            dataBuilder.Replace("$uri", httpContext.Request.GetEncodedPathAndQuery());
            dataBuilder.Replace("$scheme", httpContext.Request.Scheme);
            dataBuilder.Replace("$path", httpContext.Request.Path.Value);
            dataBuilder.Replace("$contenttype", httpContext.Request.ContentType);
            dataBuilder.Replace("$method", httpContext.Request.Method);
            dataBuilder.Replace("$host", httpContext.Request.Host.Value);
            dataBuilder.Replace("$remoteaddr", httpContext.Connection.RemoteIpAddress.ToString());
            dataBuilder.Replace("$remoteport", httpContext.Connection.RemotePort.ToString());
            return dataBuilder.ToString();
        }
    }
}