using System;
using HtcSharp.HttpModule.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IMiddlewareBuilder {

        public IServiceProvider ApplicationServices { get; }

        public IMiddlewareBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);

        public RequestDelegate Build();

    }
}