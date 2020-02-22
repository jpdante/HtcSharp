using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CorePluginLoader;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Module.Abstractions;
using HtcSharp.Core.Utils;

namespace HtcSharp.Core.Module {
    public class ModuleManager {

        private readonly HtcServer _htcServer;
        private readonly ILogger _logger;
        private readonly PluginLoader<IModule> _moduleLoader;
        private readonly Dictionary<string, IModule> _modules;

        public ModuleManager(HtcServer htcServer, ILogger logger) {
            _htcServer = htcServer;
            _logger = logger;
            _moduleLoader = new PluginLoader<IModule>();
            _modules = new Dictionary<string, IModule>();
        }

        public string[] SearchModules(string modulesPath) {
            return FileUtils.GetFiles(modulesPath, "*.module.dll", SearchOption.AllDirectories);
        }

        public void LoadModules(string modulesPath) {
            foreach (var plugin in _moduleLoader.LoadPlugins(SearchModules(modulesPath))) {
                _modules.Add(plugin.Name, plugin);
            }
        }

        public void LoadModule(string modulePath) {
            if (!File.Exists(modulePath)) return;
            foreach (var plugin in _moduleLoader.LoadPlugin(modulePath)) {
                if(!plugin.IsCompatible(HtcVersion.Major, HtcVersion.Minor, HtcVersion.Patch)) continue;
                _modules.Add(plugin.Name, plugin);
            }
        }

        public async Task UnLoadModule(string name) {
            if (_modules.TryGetValue(name, out var module)) {
                await module.Disable();
            }
        }

        public async Task CallLoad() {
            foreach (var module in _modules.Values) {
                await module.Load(_htcServer, _logger);
            }
        }

        public async Task CallEnable() {
            foreach (var module in _modules.Values) {
                await module.Enable();
            }
        }

        public async Task CallDisable() {
            foreach (var module in _modules.Values) {
                await module.Disable();
            }
        }

    }
}
