using System.IO;
using HtcPlugin.Lua.Processor.Models;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Routing;
using Microsoft.Extensions.Logging;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor {
    public class LuaProcessor : IPlugin, IHttpEvents {
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
            Logger<>.Warn($"A custom page was called. This should not happen! {{FileName: \"{filename}\"}}");
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