using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Managers;
using HtcSharp.Core.Models.Http;

namespace HtcLuaProcessor {
    public class HtcLuaProcessor : IHtcPlugin, IHttpEvents {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        public string PluginName => "HtcLuaProcessor";
        public string PluginVersion => "1.1";
        
        public void OnLoad() {
        }

        public void OnEnable() {
            UrlMapper.RegisterPluginExtension(".lua", this);
            UrlMapper.RegisterIndexFile("index.lua");
        }

        public void OnDisable() {
            UrlMapper.UnRegisterPluginExtension(".lua");
            UrlMapper.UnRegisterIndexFile("index.lua");
        }

        public bool OnHttpPageRequest(HtcHttpContext httpContext, string filename) {
            Logger.Warn($"A custom page was called. This should not happen! {{FileName: \"{filename}\"}}");
            httpContext.ErrorMessageManager.SendError(httpContext, 500);
            return false;
        }

        public bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension) {
            if (extension != ".lua") return true;
            return LuaRequest.Request(filename, httpContext);
        }
    }
}