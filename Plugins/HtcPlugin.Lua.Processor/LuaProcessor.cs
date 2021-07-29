using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor {
    public class LuaProcessor : IPlugin, IExtensionProcessor {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "HtcLuaProcessor";
        public string Version => "1.0.0";

        public Task Load() {
            Logger.LogInfo("Loading...");
            this.RegisterExtensionProcessor(".lua", this);
            return Task.CompletedTask;
        }

        public Task Enable() {
            Logger.LogInfo("Enabling...");
            return Task.CompletedTask;
        }

        public Task Disable() {
            Logger.LogInfo("Disabling...");
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        public Task OnHttpExtensionProcess(DirectiveDelegate next, HtcHttpContext httpContext, string extension) {
            Logger.LogDebug($"{httpContext.Request.Path.Value} {extension}");
            return next(httpContext);
        }
    }
}