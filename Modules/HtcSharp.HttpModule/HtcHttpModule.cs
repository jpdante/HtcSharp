using System.IO;
using System.Reflection;
using System.Text.Json;
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
            //await _httpEngine.Load();
        }

        public async Task Enable() {
            await using var fs = new FileStream("config/http.json", FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            var jsonDocument = await JsonDocument.ParseAsync(fs);
            foreach (var data in jsonDocument.RootElement.EnumerateObject()) {
                Logger.LogInfo(data.Name);
                Logger.LogInfo(data.Value[0].ToString());
            }
            //await _httpEngine.Start();
        }

        public async Task Disable() {
            //await _httpEngine.Stop();
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }
    }
}