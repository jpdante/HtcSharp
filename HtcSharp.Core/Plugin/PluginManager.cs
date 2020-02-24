using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CorePluginLoader;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Module.Abstractions;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.Core.Utils;

namespace HtcSharp.Core.Plugin {
    public class PluginManager {

        private readonly ILogger _logger;
        private readonly PluginLoader<IPlugin> _pluginLoader;
        private readonly Dictionary<string, IPlugin> _modules;

        public PluginManager(ILogger logger) {
            _logger = logger;
            _pluginLoader = new PluginLoader<IPlugin>();
            _modules = new Dictionary<string, IPlugin>();
        }

        public string[] SearchPlugins(string modulesPath) {
            return FileUtils.GetFiles(modulesPath, "*.module.dll", SearchOption.AllDirectories);
        }

        public void LoadPlugins(string modulesPath) {
            foreach (var plugin in _pluginLoader.LoadPlugins(SearchPlugins(modulesPath))) {
                _modules.Add(plugin.Name, plugin);
            }
        }

        public void LoadPlugin(string modulePath) {
            if (!File.Exists(modulePath)) return;
            foreach (var plugin in _pluginLoader.LoadPlugin(modulePath)) {
                if (!plugin.IsCompatible(HtcVersion.Major, HtcVersion.Minor, HtcVersion.Patch)) continue;
                _modules.Add(plugin.Name, plugin);
            }
        }

        public async Task UnLoadPlugin(string name) {
            if (_modules.TryGetValue(name, out var module)) {
                await module.Disable();
            }
        }

        public async Task CallLoad() {
            foreach (var plugin in _modules.Values) {
                await plugin.Load(_logger);
            }
        }

        public async Task CallEnable() {
            foreach (var plugin in _modules.Values) {
                await plugin.Enable();
            }
        }

        public async Task CallDisable() {
            foreach (var plugin in _modules.Values) {
                await plugin.Disable();
            }
        }

    }
}
