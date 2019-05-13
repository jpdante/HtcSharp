using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using HtcPlugin.LuaProcessor.Lua;
using HtcSharp.Core;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Managers;
using HtcSharp.Core.Models.Http;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.LuaProcessor {
    public class HtcLuaProcessor : IHtcPlugin, IHttpEvents {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        public string PluginName => "HtcLuaProcessor";
        public string PluginVersion => "1.1";
        public readonly LuaLowLevelAccess LuaLowLevelAccess;
        public static HtcLuaProcessor Context;

        public HtcLuaProcessor() {
            LuaLowLevelAccess = new LuaLowLevelAccess();
            Context = this;
        }

        public void OnLoad() {
            UserData.RegisterType<LuaLowLevelAccess>();
            UserData.RegisterType<LuaLowLevelClass>();
        }

        public void OnEnable() {
            UrlMapper.RegisterPluginExtension(".lua", this);
            UrlMapper.RegisterIndexFile("index.lua");
            var plugins =  HtcServer.Context.PluginsManager.LoadPluginsCustom(HtcServer.Context.PluginsPath, "*.luaplugin.dll", new []{typeof(LuaLowLevelClass), typeof(LuaLowLevelAccess), typeof(HtcLuaProcessor)});
            Context = new HtcLuaProcessor();
            foreach (var plugin in plugins) {
                plugin.OnLoad();
                plugin.OnEnable();
            }
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
            var sessionKey = LuaLowLevelAccess.StartSession();
            luaScript.Globals["__HtcLowLevel"] = LuaLowLevelAccess;
            luaScript.Globals["__HtcLowLevelSession"] = sessionKey;
            var luaIncludePath = Path.GetDirectoryName(filename).Replace(@"\", "/");
            ((ScriptLoaderBase) luaScript.Options.ScriptLoader).ModulePaths = new[] { $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua", };
            var result = LuaRequest.Request(httpContext, luaScript, filename);
            LuaLowLevelAccess.CloseSession(sessionKey);
            return result;
        }
    }
}