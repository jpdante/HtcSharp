using System.Threading.Tasks;
using HtcSharp.Abstractions;

namespace HtcSharp.HttpModule {
    public class HtcHttpModule : IModule {

        public string Name => "HtcHttp";
        public string Version => "0.1.0";

        public Task Load() {
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