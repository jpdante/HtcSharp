using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Middleware;

namespace HtcSharp.HttpModule.StaticFiles {
    public class StaticFileMiddleware : IMiddleware {

        private readonly RequestDelegate _next;

        public StaticFileMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HtcHttpContext httpContext) {
            await TryServeFile(httpContext);
            await _next(httpContext);
        }

        public async Task TryServeFile(HtcHttpContext httpContext) {

        }
    }
}