using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcPlugin.Lua.Processor.Core;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;

namespace HtcPlugin.Lua.Processor {
    public class LuaProcessor : IPlugin, IExtensionProcessor {

        internal static readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "HtcLuaProcessor";
        public string Version => "1.0.0";

        public Task Init(IServiceProvider serviceProvider) {
            Logger.LogInfo("Loading...");
            this.RegisterExtensionProcessor(".lua", this);
            this.RegisterIndex("index.lua");
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

        public bool IsCompatible(IVersion version) {
            return true;
        }

        public Task OnHttpExtensionProcess(DirectiveDelegate next, HtcHttpContext httpContext, string fileName, string extension) {
            var luaContext = new LuaContext(httpContext, fileName);
            return luaContext.LoadFile() ? luaContext.ProcessRequest() : next(httpContext);
        }

        public void Dispose() {
        }
    }
}