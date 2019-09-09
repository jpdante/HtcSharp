using System.IO;
using System.Reflection;
using HtcPlugin.Lua.Processor.Models;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor {
    public class LuaProcessor : IHtcPlugin, IHttpEvents {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        public static LuaProcessor Context;
        public string PluginName => "HtcLuaProcessor";
        public string PluginVersion => "0.1.2";
        public readonly LuaLowLevelAccess LuaLowLevelAccess;
        public readonly CustomLuaLoader ScriptLoader;

        public LuaProcessor() {
            Context = this;
            LuaLowLevelAccess = new LuaLowLevelAccess();
            ScriptLoader = new CustomLuaLoader();
        }

        public void OnLoad() {
            UserData.RegisterType<LuaLowLevelAccess>();
            UserData.RegisterType<LuaLowLevelClass>();
        }

        public void OnEnable() {
            UrlMapper.RegisterPluginExtension(".lua", this);
            UrlMapper.RegisterIndexFile("index.lua");
            ScriptLoader.Initialize();
        }

        public void OnDisable() {
            UrlMapper.UnRegisterPluginExtension(".lua");
            UrlMapper.UnRegisterIndexFile("index.lua");
            LuaLowLevelAccess.ClearLowLevelClasses();
        }

        public bool OnHttpPageRequest(HtcHttpContext httpContext, string filename) {
            Logger.Warn($"A custom page was called. This should not happen! {{FileName: \"{filename}\"}}");
            httpContext.ErrorMessageManager.SendError(httpContext, 500);
            return false;
        }

        public bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension) {
            if (extension != ".lua") return true;
            var luaScript = LuaRequest.NewScript();
            luaScript.Options.ScriptLoader = ScriptLoader;
            luaScript.Globals["__HtcLowLevel"] = LuaLowLevelAccess;
            var luaIncludePath = Path.GetDirectoryName(filename).Replace(@"\", "/");
            ((ScriptLoaderBase) luaScript.Options.ScriptLoader).ModulePaths = new[] { $"?.lua", $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua", };
            var result = LuaRequest.Request(httpContext, luaScript, filename);
            return result;
        }
    }
}