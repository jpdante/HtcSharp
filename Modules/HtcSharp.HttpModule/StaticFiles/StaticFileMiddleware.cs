using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using IMiddleware = HtcSharp.HttpModule.Abstractions.IMiddleware;
using RequestDelegate = HtcSharp.HttpModule.Middleware.RequestDelegate;

namespace HtcSharp.HttpModule.StaticFiles {
    public class StaticFileMiddleware : IMiddleware {

        private readonly RequestDelegate _next;

        private readonly string _path;

        public StaticFileMiddleware(RequestDelegate next, string path) {
            _next = next;
            _path = path;
        }

        public Task Invoke(HtcHttpContext httpContext) {
            if (!IsHeadOrGet(httpContext)) {
            } else if (!GetPath(httpContext, "", out var subPath)) {
            } else if (!GetContentType(subPath, out string contentType)) {
            } else {
                return TryServeFile(httpContext, subPath, contentType);
            }
            return _next(httpContext);
        }

        private static bool IsHeadOrGet(HttpContext httpContext) {
            return HttpMethods.IsHead(httpContext.Request.Method) || HttpMethods.IsGet(httpContext.Request.Method);
        }

        private static bool GetPath(HttpContext context, PathString matchUrl, out PathString subPath, bool forDirectory = false) {
            var path = context.Request.Path;

            if (forDirectory && !PathEndsInSlash(path)) {
                path += new PathString("/");
            }

            return path.StartsWithSegments(matchUrl, out subPath);
        }

        private static bool PathEndsInSlash(PathString path) {
            return path.Value.EndsWith("/", StringComparison.Ordinal);
        }

        private static bool GetContentType(PathString subPath, out string contentType) {
            var rawContentType = ContentType.CUSTOM_BINARY.FromExtension(subPath.Value);
            if (rawContentType == 0) rawContentType = ContentType.CUSTOM_BINARY;
            contentType = rawContentType.ToValue();
            return true;
        }

        public async Task TryServeFile(HtcHttpContext httpContext, PathString subPath, string contentType) {
            
        }
    }
}