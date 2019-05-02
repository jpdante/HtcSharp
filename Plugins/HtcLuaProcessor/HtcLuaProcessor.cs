using System.Reflection;
using System.Threading;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;

namespace HtcLuaProcessor {
    public class HtcLuaProcessor : IHtcPlugin, IHttpEvents {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        public string PluginName => "HtcLuaProcessor";
        public string PluginVersion => "1.0";
        
        public void OnLoad() {
        }

        public void OnEnable() {
            UrlMapper.RegisterIndexFile(".lua");
        }

        public void OnDisable() {
            UrlMapper.UnRegisterIndexFile(".lua");
        }

        public bool OnHttpPageRequest(HtcHttpContext context, string filename) {
            return false;
        }

        public bool OnHttpExtensionRequest(HtcHttpContext context, string filename, string extension) {
            if (extension != ".lua") return false;

            return false;
        }
    }
}