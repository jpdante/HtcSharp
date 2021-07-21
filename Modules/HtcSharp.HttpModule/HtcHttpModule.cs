using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule.Config;
using HtcSharp.Logging;
using HtcSharp.Shared.IO;

namespace HtcSharp.HttpModule {
    public class HtcHttpModule : IModule {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "HtcHttp";
        public string Version => "0.1.0";

        private HttpModuleConfig _config;
        private HttpEngine _httpEngine;

        public async Task Load() {
            _config = await LoadConfig();
            _httpEngine = new HttpEngine(Path.GetFullPath(_config.SitesPath));
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

        public async Task<HttpModuleConfig> LoadConfig() {
            string fileName = Path.Combine(PathExt.GetConfigPath(true, "http"), "config.json");
            if (File.Exists(fileName)) {
                await using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                return await JsonSerializer.DeserializeAsync<HttpModuleConfig>(fileStream);
            } else {
                await using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                var config = new HttpModuleConfig();
                await JsonSerializer.SerializeAsync(fileStream, config, new JsonSerializerOptions() {
                    WriteIndented = true
                });
                return config;
            }
        }
    }
}