using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Shared;
using HtcSharp.HttpModule.Routing;
using HtcSharp.HttpModule.Shared;

namespace HtcSharp.HttpModule.IO {
    public static class HttpIO {
        public static string ReplaceVars(HttpContext httpContext, string data) {
            data = data.Replace("$scheme", httpContext.Request.Scheme);
            data = data.Replace("$uri", httpContext.Request.RequestPath);
            data = data.Replace("$host", httpContext.Request.Host.ToString());
            return data;
        }
    }
}