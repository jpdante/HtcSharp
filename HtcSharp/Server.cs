using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.Core;
using HtcSharp.Core.Module;
using HtcSharp.Core.Plugin;
using HtcSharp.Internal;
using HtcSharp.Logging;
using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Internal;

namespace HtcSharp {
    public class Server : DaemonApplication {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private ArgsReader ArgsReader;
        private Config Config;
        private ModuleManager ModuleManager;
        private PluginManager PluginManager;

        protected override async Task OnLoad() {
            var multiAppender = new MultiAppender();
            multiAppender.AddAppender(new ConsoleAppender(LogLevel.All));
            multiAppender.AddAppender(new RollingFileAppender(new RollingFileAppender.RollingFileConfig(), LogLevel.All));
            LoggerManager.Init(multiAppender);

            Logger.LogInfo("Loading...");

            ArgsReader = new ArgsReader(Args);
            await LoadConfig();

            ModuleManager = new ModuleManager();
            await ModuleManager.LoadModules(Path.GetFullPath(Config.ModulesPath));

            PluginManager = new PluginManager();
            await PluginManager.LoadPlugins(Path.GetFullPath(Config.ModulesPath));
        }

        private async Task LoadConfig() {
            string configPath = ArgsReader.GetOrDefault("config", "./config.json");
            configPath = Path.GetFullPath(configPath);
            if (File.Exists(configPath)) {
                await using var fileStream = new FileStream(configPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                Config = await JsonSerializer.DeserializeAsync<Config>(fileStream);
            } else {
                await using var fileStream = new FileStream(configPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                Config = new Config();
                await JsonSerializer.SerializeAsync(fileStream, Config, new JsonSerializerOptions {
                    WriteIndented = true
                });
            }
        }

        protected override Task OnStart() {
            Logger.LogInfo("Starting...");
            return Task.CompletedTask;
        }

        protected override async Task OnExit() {
            await PluginManager.UnloadPlugins();
            await ModuleManager.UnloadModules();
            Logger.LogInfo("Exiting...");
        }

    }
}