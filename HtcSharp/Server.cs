using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Commands;
using HtcSharp.Core;
using HtcSharp.Core.Cli;
using HtcSharp.Core.Module;
using HtcSharp.Core.Plugin;
using HtcSharp.Internal;
using HtcSharp.Logging;
using HtcSharp.Shared.IO;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HtcSharp {
    public class Server : DaemonApplication, IServer {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        internal readonly Version version;
        internal Config config;
        internal ArgsReader argsReader;
        internal ModuleManager moduleManager;
        internal PluginManager pluginManager;
        internal CliServer cliServer;

        public Server() {
            version = new Version {
                Major = 1,
                Minor = 0,
                Patch = 0,
            };
        }

        protected override async Task OnLoad() {
            PathExt.EnsureFolders();

            argsReader = new ArgsReader(Args);

            config = await LoadConfig(argsReader.GetOrDefault("config", "./config.yml"));

            LoggerManager.Dispose();
            LoggerManager.Init(config.Logging.GetAppender());

            Logger.LogInfo("Loading...");

            cliServer = new CliServer(argsReader.GetOrDefault("pipe", "htcsharp"));
            cliServer.AddCommand(new ReloadCommand(this));

            moduleManager = new ModuleManager(version, configureServices => {
                configureServices.AddSingleton(cliServer);
            });
            await moduleManager.LoadModules(Path.GetFullPath(config.ModulesPath));

            await moduleManager.InitModules();

            pluginManager = new PluginManager(version, moduleManager, configureServices => {
                configureServices.AddSingleton(cliServer);
            });
            await pluginManager.LoadPlugins(Path.GetFullPath(config.PluginsPath));

            await pluginManager.InitPlugins();
        }

        protected override async Task OnStart() {
            Logger.LogInfo("Starting...");

            await moduleManager.EnableModules();
            await pluginManager.EnablePlugins();

            await cliServer.Start();
        }

        protected override async Task OnExit() {
            await cliServer.Stop();

            await pluginManager.DisablePlugins();
            await moduleManager.DisableModules();

            await pluginManager.UnloadPlugins();
            await moduleManager.UnloadModules();

            Logger.LogInfo("Exiting...");
        }

        public async Task OnReload() {
            Logger.LogInfo("Reloading...");

            await pluginManager.DisablePlugins();
            await moduleManager.DisableModules();

            await pluginManager.UnloadPlugins();
            await moduleManager.UnloadModules();

            LoggerManager.Dispose();
            config = await LoadConfig(argsReader.GetOrDefault("config", "./config.yml"));
            LoggerManager.Init(config.Logging.GetAppender());

            await moduleManager.LoadModules(Path.GetFullPath(config.ModulesPath));
            await moduleManager.InitModules();

            await pluginManager.LoadPlugins(Path.GetFullPath(config.PluginsPath));
            await pluginManager.InitPlugins();

            await moduleManager.EnableModules();
            await pluginManager.EnablePlugins();
            Logger.LogInfo("Reloaded!");
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