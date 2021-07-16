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

        public TestMiddleware(string name) {
            Name = name;
        }

        public Task<bool> Invoke(HtcHttpContext httpContext) {
            Logger.LogInfo($"Passing middlware: {Name}");
            return Task.FromResult(true);
        }

    }
}