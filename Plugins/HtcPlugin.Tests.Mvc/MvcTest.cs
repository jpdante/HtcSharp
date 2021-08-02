using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Mvc;
using HtcSharp.Logging;

namespace HtcPlugin.Tests.Mvc {
    public class MvcTest : HttpMvc, IPlugin {

        internal static readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "MvcTest";
        public string Version => "1.0.0";

        public Task Load() {
            Logger.LogInfo("Loading...");
            LoadControllers(GetType().Assembly);
            this.RegisterMvc(this);
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
    }
}
