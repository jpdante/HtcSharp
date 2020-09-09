using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public static class StringHttpExtensions {
        public static string ReplaceHttpContextVars(this string data, HttpContext httpContext) {
            data = data.Replace("$scheme", httpContext.Request.Scheme);
            data = data.Replace("$uri", httpContext.Request.RequestPath);
            data = data.Replace("$host", httpContext.Request.Host.ToString());
            return data;
        }
    }
}