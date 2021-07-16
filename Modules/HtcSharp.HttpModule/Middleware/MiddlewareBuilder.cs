using System.Collections.Generic;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Middleware.Internal;

namespace HtcSharp.HttpModule.Middleware {
    public class MiddlewareBuilder {

        private readonly List<IMiddleware> _middlewares;

        public MiddlewareBuilder() {
            _middlewares = new List<IMiddleware>();
        }

        public MiddlewareBuilder UseRewriter() {
            var middleware = new TestMiddleware("Rewriter");
            _middlewares.Add(middleware);
            return this;
        }

        public MiddlewareBuilder UseRouter() {
            var middleware = new TestMiddleware("Router");
            _middlewares.Add(middleware);
            return this;
        }

        public MiddlewareBuilder UseMvc() {
            var middleware = new TestMiddleware("Mvc");
            _middlewares.Add(middleware);
            return this;
        }

        public MiddlewareBuilder UsePages() {
            var middleware = new TestMiddleware("Pages");
            _middlewares.Add(middleware);
            return this;
        }

        public MiddlewareBuilder UseHttpEvents() {
            var middleware = new TestMiddleware("HttpEvents");
            _middlewares.Add(middleware);
            return this;
        }

        public MiddlewareBuilder UseStaticFiles() {
            var middleware = new TestMiddleware("StaticFiles");
            _middlewares.Add(middleware);
            return this;
        }

        public MiddlewareContext Build() {
            return new MiddlewareContext(_middlewares);
        }

    }
}