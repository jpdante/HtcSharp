using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Core;
using HtcSharp.Core.Console;
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

        internal readonly Version Version;
        internal Config Config;
        internal ArgsReader ArgsReader;
        internal ModuleManager ModuleManager;
        internal PluginManager PluginManager;
        internal CliServer CliServer;

        public Server() {
            Version = new Version {
                Major = 1,
                Minor = 0,
                Patch = 0,
            };
        }

        protected override async Task OnLoad() {
            PathExt.EnsureFolders();

            ArgsReader = new ArgsReader(Args);

            Config = await LoadConfig(ArgsReader.GetOrDefault("config", "./config.yml"));

            LoggerManager.Dispose();
            LoggerManager.Init(Config.Logging.GetAppender());

            Logger.LogInfo("Loading...");

            CliServer = new CliServer(ArgsReader.GetOrDefault("pipe", "htcsharp"));

            ModuleManager = new ModuleManager(Version, configureServices => {

            });
            await ModuleManager.LoadModules(Path.GetFullPath(Config.ModulesPath));

            await ModuleManager.InitModules();

            PluginManager = new PluginManager(Version, ModuleManager, configureServices => {

            });
            await PluginManager.LoadPlugins(Path.GetFullPath(Config.PluginsPath));

            await PluginManager.InitPlugins();
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

            await ModuleManager.DisableModules();
            await PluginManager.DisablePlugins();

            Config = await LoadConfig(ArgsReader.GetOrDefault("config", "./config.yml"));
            LoggerManager.Init(Config.Logging.GetAppender());

            await ModuleManager.InitModules();
            await PluginManager.InitPlugins();

            await ModuleManager.EnableModules();
            await PluginManager.EnablePlugins();
        }

        private async Task<Config> LoadConfig(string configPath) {
            configPath = Path.GetFullPath(configPath);
            if (File.Exists(configPath)) {
                await using var fileStream = new FileStream(configPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();
                return deserializer.Deserialize<Config>(streamReader);
            } else {
                await using var fileStream = new FileStream(configPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                await using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                var config = new Config();
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();
                serializer.Serialize(streamWriter, config);
                return config;
            }
        }
    }
}