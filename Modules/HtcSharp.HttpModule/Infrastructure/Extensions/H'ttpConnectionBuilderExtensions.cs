using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Http.Http.Abstractions;

namespace HtcSharp.HttpModule.Infrastructure.Extensions {
    internal static class HttpConnectionBuilderExtensions {
        public static IConnectionBuilder UseHttpServer<TContext>(this IConnectionBuilder builder, ServiceContext serviceContext, IHttpApplication<TContext> application, HttpProtocols protocols) {
            var middleware = new HttpConnectionMiddleware<TContext>(serviceContext, application, protocols);
            return builder.Use(next => {
                return middleware.OnConnectionAsync;
            });
        }
    }
}