using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class TestDirective : IDirective {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name { get; set; }

        private readonly DirectiveDelegate _next;

        public TestDirective(DirectiveDelegate next, JsonElement config) {
            Name = config.GetProperty("name").GetString();
            _next = next;
        }

        public Task Invoke(HtcHttpContext httpContext) {
            Logger.LogDebug($"Passing directive: {Name}");
            return _next(httpContext);
        }

    }
}