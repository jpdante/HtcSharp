using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Middleware {
    public class MiddlewareContext {

        private readonly IReadOnlyList<IMiddleware> _middlewares;

        internal MiddlewareContext(IReadOnlyList<IMiddleware> middlewares) {
            _middlewares = middlewares;
        }

        public async Task Invoke(HtcHttpContext httpContext) {
            foreach (var middleware in _middlewares) {
                bool cancel = await middleware.Invoke(httpContext);
                if (!cancel) break;
            }
        }
    }
}