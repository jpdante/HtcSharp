using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CorePluginLoader;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.Core.Utils;

namespace HtcSharp.Core.Plugin {
    public class PluginManager {

        private readonly ILogger _logger;
        private readonly PluginLoader<IPlugin> _pluginLoader;
        private readonly Dictionary<string, IPlugin> _plugins;
        private readonly PluginServerContext _pluginServerContext;

        public PluginManager(PluginServerContext pluginServerContext, ILogger logger) {
            _logger = logger;
            _pluginLoader = new PluginLoader<IPlugin>();
            _plugins = new Dictionary<string, IPlugin>();
            _pluginServerContext = pluginServerContext;
        }

        public string[] SearchPlugins(string modulesPath) {
            return FileUtils.GetFiles(modulesPath, "*.module.dll", SearchOption.AllDirectories);
        }

        public void LoadPlugins(string modulesPath) {
            foreach (var plugin in _pluginLoader.LoadPlugins(SearchPlugins(modulesPath))) {
                _plugins.Add(plugin.Name, plugin);
            }
        }

        public void LoadPlugin(string modulePath) {
            if (!File.Exists(modulePath)) return;
            foreach (var plugin in _pluginLoader.LoadPlugin(modulePath)) {
                if (!plugin.IsCompatible(HtcVersion.Major, HtcVersion.Minor, HtcVersion.Patch)) continue;
                _plugins.Add(plugin.Name, plugin);
            }
        }

        public async Task UnLoadPlugin(string name) {
            if (_plugins.TryGetValue(name, out var module)) {
                await module.Disable();
            }
        }

        public IPlugin[] GetPlugins() {
            return _plugins.Values.Select(c => c).ToArray();
        }

        public async Task CallLoad() {
            foreach (var plugin in _plugins.Values) {
                _logger.LogInfo($"Loading Plugin {plugin.Name} {plugin.Version}...", null);
                await plugin.Load(_pluginServerContext, _logger);
            }
        }

        public async Task CallEnable() {
            foreach (var plugin in _plugins.Values) {
                _logger.LogInfo($"Enabling Plugin {plugin.Name} {plugin.Version}...", null);
                await plugin.Enable();
            }
        }

        public async Task CallDisable() {
            foreach (var plugin in _plugins.Values) {
                _logger.LogInfo($"Disabling Plugin {plugin.Name} {plugin.Version}...", null);
                await plugin.Disable();
            }
        }

    }
}
