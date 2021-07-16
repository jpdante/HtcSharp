using System.Collections.Generic;
using HtcSharp.HttpModule.Abstractions;

namespace HtcSharp.HttpModule.Middleware {
    public class MiddlewareBuilder {

        private readonly List<IMiddleware> _middlewares;

        public MiddlewareBuilder() {
            _middlewares = new List<IMiddleware>();
        }

        public MiddlewareBuilder UseRewriter() {
            // TODO
            return this;
        }

        public MiddlewareBuilder UseRouter() {
            // TODO
            return this;
        }

        public MiddlewareBuilder UseMvc() {
            // TODO
            return this;
        }

        public MiddlewareBuilder UsePages() {
            // TODO
            return this;
        }

        public MiddlewareBuilder UsePlugins() {
            // TODO
            return this;
        }

        public MiddlewareContext Build() {
            return new MiddlewareContext();
        }

    }
}