using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcPlugin.Tests.Mvc {
    public class MvcTest : HttpMvc, IPlugin {

        public string Name => "MvcTest";
        public string Version => "1.0.2";

        private ILogger _logger;

        public Task Disable() {
            return Task.CompletedTask;
        }

        public Task Enable() {
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        public Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            _logger = logger;
            Setup(Assembly.GetExecutingAssembly(), _logger);
            return Task.CompletedTask;
        }

        public override Task<bool> BeforePageRequest(HttpContext httpContext, string filename) {
            _logger.LogInfo($"Request: {filename}");
            return Task.FromResult(false);
        }
    }
}
