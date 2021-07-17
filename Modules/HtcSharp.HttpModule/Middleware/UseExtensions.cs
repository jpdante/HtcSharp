using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Middleware.Internal;

namespace HtcSharp.HttpModule.Middleware {
    public static class UseExtensions {
        
        public static IMiddlewareBuilder UseRewriter(this IMiddlewareBuilder middlewareBuilder) {
            return middlewareBuilder.UseMiddleware<TestMiddleware>("Rewriter");
        }

        public static IMiddlewareBuilder UseRouter(this IMiddlewareBuilder middlewareBuilder) {
            return middlewareBuilder.UseMiddleware<TestMiddleware>("Router");
        }

        public static IMiddlewareBuilder UseMvc(this IMiddlewareBuilder middlewareBuilder) {
            return middlewareBuilder.UseMiddleware<TestMiddleware>("Mvc");
        }

        public static IMiddlewareBuilder UsePages(this IMiddlewareBuilder middlewareBuilder) {
            return middlewareBuilder.UseMiddleware<TestMiddleware>("Pages");
        }

        public static IMiddlewareBuilder UseHttpEvents(this IMiddlewareBuilder middlewareBuilder) {
            return middlewareBuilder.UseMiddleware<TestMiddleware>("HttpEvents");
        }

        public static IMiddlewareBuilder UseStaticFiles(this IMiddlewareBuilder middlewareBuilder) {
            return middlewareBuilder.UseMiddleware<TestMiddleware>("StaticFiles");
        }

    }
}