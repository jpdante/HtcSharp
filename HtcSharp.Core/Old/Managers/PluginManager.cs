using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CorePluginLoader;
using HtcSharp.Core.Old.Interfaces.Plugin;
using HtcSharp.Core.Old.Logging;
using HtcSharp.Core.Old.Utils;

namespace HtcSharp.Core.Old.Managers {
    public class PluginManager {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<IHtcPlugin> _plugins;

        public PluginManager() {
            _plugins = new List<IHtcPlugin>();
        }

        public void Call_OnLoad() {
            foreach (var plugin in _plugins) {
                Logger.Info($"Loading: {plugin.PluginName}({plugin.PluginVersion})");
                plugin.OnLoad();
            }
        }

        public void Call_OnEnable() {
            foreach (var plugin in _plugins) {
                plugin.OnEnable();
                Logger.Info($"Enabled: {plugin.PluginName}({plugin.PluginVersion})");
            }
        }

        public void Call_OnDisable() {
            foreach (var plugin in _plugins) {
                plugin.OnDisable();
                Logger.Info($"Disabled: {plugin.PluginName}({plugin.PluginVersion})");
            }
        }

        public void UnloadPlugin(IHtcPlugin plugin) {
            _plugins.Remove(plugin);
            plugin.OnDisable();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void LoadPlugins(string pluginsPath) {
            if(!Directory.Exists(pluginsPath)) {
                Logger.Fatal($"Plugins path '{pluginsPath}' does not exist!");
                Environment.Exit(2);
            }
            var pluginLoader = new PluginLoader<IHtcPlugin>();
            foreach (var plugin in pluginLoader.LoadPlugins(IoUtils.GetFilesExceptionFix(pluginsPath, "*.plugin.dll", SearchOption.AllDirectories))) {
                _plugins.Add(plugin);
            }
        }

        public IEnumerable<IHtcPlugin> LoadPluginsCustom(string pluginsPath, string searchPattern) {
            if(!Directory.Exists(pluginsPath)) {
                Logger.Fatal($"Plugins path '{pluginsPath}' does not exist!");
            }
            var pluginLoader = new PluginLoader<IHtcPlugin>();
            return pluginLoader.LoadPlugins(IoUtils.GetFilesExceptionFix(pluginsPath, searchPattern, SearchOption.AllDirectories));
        }

        public IHtcPlugin[] GetPlugins => _plugins.ToArray();
    }
}
