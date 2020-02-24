using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using HtcSharp.Core.Engine;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Module;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Utils;

namespace HtcSharp.Core {
    public class HtcServer {

        private readonly JObject _config;
        private readonly ModuleManager _moduleManager;
        private readonly PluginManager _pluginManager;
        private readonly PluginServerContext _pluginServerContext;

        public readonly EngineManager EngineManager;
        public readonly MultiLogger Logger;
        public bool Running { get; private set; }

        public HtcServer(string configPath) : this(JsonUtils.GetJsonFile(configPath)) { }

        public HtcServer(JObject config) {
            _config = config;
            Logger = new MultiLogger();
            EngineManager = new EngineManager(Logger);
            _moduleManager = new ModuleManager(this, Logger);
            _pluginServerContext = new PluginServerContext(HtcIOUtils.ReplacePathTags(_config.GetValue("PluginsPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>()), _pluginManager);
            _pluginManager = new PluginManager(_pluginServerContext, Logger);
            Running = false;
        }

        public async Task Start() {
            if (Running) return;
            Running = true;
            Logger.LogInfo("Loading HtcServer...", null);
            Logger.LogInfo("Loading Modules...", null);
            _moduleManager.LoadModules(HtcIOUtils.ReplacePathTags(_config.GetValue("ModulesPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>()));
            await _moduleManager.CallLoad();
            Logger.LogInfo("Loading Engines...", null);
            var enginesConfigs = (JObject)_config.GetValue("Engines", StringComparison.CurrentCultureIgnoreCase);
            foreach (string engineName in EngineManager.GetEnginesNames()) {
                if (!enginesConfigs.ContainsKey(engineName)) continue;
                var engineConfig = (JObject) enginesConfigs.GetValue(engineName, StringComparison.CurrentCultureIgnoreCase);
                var engine = EngineManager.InstantiateEngine(engineName);
                EngineManager.AddEngine(engine);
                Logger.LogInfo($"Loading Engine {engineName}...", null);
                await EngineManager.Load(engine, engineConfig);
            }
            Logger.LogInfo("Loading Plugins...", null);
            _pluginManager.LoadPlugins(HtcIOUtils.ReplacePathTags(_config.GetValue("PluginsPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>()));
            await _pluginManager.CallLoad();
            Logger.LogInfo("Starting HtcServer...", null);
            Logger.LogInfo("Enabling Modules...", null);
            await _moduleManager.CallEnable();
            Logger.LogInfo("Starting Engines...", null);
            await EngineManager.Start();
            Logger.LogInfo("Enabling Plugins...", null);
            await _pluginManager.CallEnable();
        }

        public async Task Stop() {
            if (!Running) return;
            Logger.LogInfo("Stopping HtcServer...", null);
            Logger.LogInfo("Disabling Plugins...", null);
            await _pluginManager.CallDisable();
            Logger.LogInfo("Stopping Engines...", null);
            await EngineManager.Stop();
            Logger.LogInfo("Disabling Modules...", null);
            await _moduleManager.CallDisable();
            Running = false;
        }
    }
}
