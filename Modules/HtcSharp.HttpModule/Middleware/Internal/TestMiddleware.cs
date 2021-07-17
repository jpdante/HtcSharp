using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;

namespace HtcSharp.HttpModule.Middleware.Internal {
    public class TestMiddleware : IMiddleware {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name { get; set; }

        private readonly RequestDelegate _next;

        public TestMiddleware(RequestDelegate next, string name) {
            Name = name;
            _next = next;
        }

        public Task Invoke(HtcHttpContext httpContext) {
            Logger.LogInfo($"Passing middlware: {Name}");
            return _next(httpContext);
        }

    }
}