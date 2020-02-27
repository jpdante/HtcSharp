using System.IO;
using System.Threading.Tasks;
using HtcPlugin.Lua.Processor.Models;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor {
    public class LuaProcessor : IPlugin, IHttpEvents {
        public string Name => "HtcLuaProcessor";
        public string Version => "0.1.2";

        public static LuaProcessor Context;
        public ILogger Logger;
        public readonly LuaLowLevelAccess LuaLowLevelAccess;
        public readonly CustomLuaLoader ScriptLoader;

        public LuaProcessor() {
            Context = this;
            LuaLowLevelAccess = new LuaLowLevelAccess();
            ScriptLoader = new CustomLuaLoader();
        }

        public Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            Logger = logger;
            UserData.RegisterType<LuaLowLevelAccess>();
            UserData.RegisterType<LuaLowLevelClass>();
            return Task.CompletedTask;
        }

        public Task Enable() {
            UrlMapper.RegisterPluginExtension(".lua", this);
            UrlMapper.RegisterIndexFile("index.lua");
            ScriptLoader.Initialize();
            return Task.CompletedTask;
        }

        public Task Disable() {
            UrlMapper.UnRegisterPluginExtension(".lua");
            UrlMapper.UnRegisterIndexFile("index.lua");
            LuaLowLevelAccess.ClearLowLevelClasses();
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        public async Task OnHttpPageRequest(HttpContext httpContext, string filename) {
            Logger.LogWarn($"A custom page was called. This should not happen! {{FileName: \"{filename}\"}}");
            await httpContext.ServerInfo.ErrorMessageManager.SendError(httpContext, 500);
        }

        public Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension) {
            if (extension != ".lua") return Task.CompletedTask;
            var luaScript = LuaRequest.NewScript();
            luaScript.Options.ScriptLoader = ScriptLoader;
            luaScript.Globals["__HtcLowLevel"] = LuaLowLevelAccess;
            string luaIncludePath = Path.GetDirectoryName(filename).Replace(@"\", "/");
            ((ScriptLoaderBase)luaScript.Options.ScriptLoader).ModulePaths = new[] { $"?.lua", $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua", };
            return LuaRequest.Request(httpContext, luaScript, filename);
        }
    }
}