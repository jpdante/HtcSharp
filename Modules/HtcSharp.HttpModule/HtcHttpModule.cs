using System.Threading.Tasks;
using HtcSharp.Core;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Module.Abstractions;

namespace HtcSharp.HttpModule {
    public class HtcHttpModule : IModule {
        public string Name => "HtcHttp";
        public string Version => "1.2.1";

        public Task Load(HtcServer htcServer, ILogger logger) {
            htcServer.EngineManager.RegisterEngine("http", typeof(HttpEngine));
            return Task.CompletedTask;
        }

        public Task Enable() {
            return Task.CompletedTask;
        }

        public Task Disable() {
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }
    }
}