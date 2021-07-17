using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Middleware {
    public class MiddlewareBuilder : IMiddlewareBuilder {

        public IServiceProvider ApplicationServices { get; }

        private readonly IList<Func<RequestDelegate, RequestDelegate>> _middlewares;

        public MiddlewareBuilder(IServiceProvider serviceProvider) {
            ApplicationServices = serviceProvider;
            _middlewares = new List<Func<RequestDelegate, RequestDelegate>>();
        }

        public IMiddlewareBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) {
            _middlewares.Add(middleware);
            return this;
        }

        public RequestDelegate Build() {
            RequestDelegate app = context =>  {
                // End of pipe-line
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            };

            foreach (Func<RequestDelegate, RequestDelegate> component in _middlewares.Reverse()) {
                app = component(app);
            }

            return app;
        }

    }
}