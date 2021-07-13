using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Logging;

namespace HtcSharp.HttpModule {
    public class HtcHttpModule : IModule {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "HtcHttp";
        public string Version => "0.1.0";

        private HttpEngine _httpEngine;

        public async Task Load() {
            _httpEngine = new HttpEngine();
            await _httpEngine.Load();
        }

        public async Task Enable() {
            await _httpEngine.Start();
        }

        public async Task Disable() {
            await _httpEngine.Stop();
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }
    }
}