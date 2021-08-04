using System.Collections.Generic;
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
        private CliServer CliServer;

        protected override async Task OnLoad() {
            PathExt.EnsureFolders();
            ArgsReader = new ArgsReader(Args);
            await LoadConfig();

            LoggerManager.Dispose();
            LoggerManager.Init(Config.Logging.GetAppender());

            Logger.LogInfo("Loading...");

            CliServer = new CliServer(ArgsReader.GetOrDefault("pipe", "htcsharp"));
            CliServer.AddCommand(new ReloadAllCommand(this));

            ModuleManager = new ModuleManager();
            await ModuleManager.FindModules(Path.GetFullPath(Config.ModulesPath));

            PluginManager = new PluginManager(ModuleManager);
            await PluginManager.FindPlugins(Path.GetFullPath(Config.PluginsPath));

            await ModuleManager.LoadModules();
            await PluginManager.LoadPlugins();
        }

        protected override async Task OnStart() {
            Logger.LogInfo("Starting...");

            await ModuleManager.EnableModules();
            await PluginManager.EnablePlugins();

            await CliServer.Start();
        }

        protected override async Task OnExit() {
            await CliServer.Stop();

            await ModuleManager.DisableModules();
            await PluginManager.DisablePlugins();

            await PluginManager.UnloadPlugins();
            await ModuleManager.UnloadModules();

            Logger.LogInfo("Exiting...");
        }

        public async Task OnReload() {
            Logger.LogInfo("Reloading...");
            LoggerManager.Dispose();

            await LoadConfig();
            LoggerManager.Init(Config.Logging.GetAppender());

            await ModuleManager.DisableModules();
            await PluginManager.DisablePlugins();

            await ModuleManager.LoadModules();
            await PluginManager.LoadPlugins();

            await ModuleManager.EnableModules();
            await PluginManager.EnablePlugins();
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