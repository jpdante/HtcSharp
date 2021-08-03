using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Abstractions.Internal;
using HtcSharp.Core;
using HtcSharp.Core.Console;
using HtcSharp.Core.Console.Commands;
using HtcSharp.Core.Module;
using HtcSharp.Core.Plugin;
using HtcSharp.Internal;
using HtcSharp.Logging;
using HtcSharp.Shared.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HtcSharp {
    public class Server : DaemonApplication, IServer {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private ArgsReader ArgsReader;
        private Config Config;
        private ModuleManager ModuleManager;
        private PluginManager PluginManager;
        private CliManager CliManager;

        protected override async Task OnLoad() {
            PathExt.EnsureFolders();
            ArgsReader = new ArgsReader(Args);
            await LoadConfig();

            LoggerManager.Init(Config.Logging.GetAppender());

            Logger.LogInfo("Loading...");

            ModuleManager = new ModuleManager();
            await ModuleManager.LoadModules(Path.GetFullPath(Config.ModulesPath));

            PluginManager = new PluginManager(ModuleManager);
            await PluginManager.LoadPlugins(Path.GetFullPath(Config.PluginsPath));

            CliManager = new CliManager("htcsharp");
            CliManager.AddCommand(new ReloadCommand(this));
        }

        protected override async Task OnStart() {
            Logger.LogInfo("Starting...");
            await ModuleManager.InitModules();
            await PluginManager.InitPlugins();
            await CliManager.Start();
        }

        protected override async Task OnExit() {
            await PluginManager.UnloadPlugins();
            await ModuleManager.UnloadModules();
            await CliManager.Stop();
            Logger.LogInfo("Exiting...");
        }

        public async Task OnReload() {
            await PluginManager.UnloadPlugins();
            await ModuleManager.UnloadModules();

            await ModuleManager.InitModules();
            await PluginManager.InitPlugins();
        }

        private async Task LoadConfig() {
            string configPath = ArgsReader.GetOrDefault("config", "./config.yml");
            configPath = Path.GetFullPath(configPath);
            if (File.Exists(configPath)) {
                await using var fileStream = new FileStream(configPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();
                Config = deserializer.Deserialize<Config>(streamReader);
            } else {
                await using var fileStream = new FileStream(configPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                await using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                Config = new Config();
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();
                serializer.Serialize(streamWriter, Config);
            }
        }
    }
}