using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.IO;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Utils;
using McMaster.NETCore.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HtcSharp.Core.Managers {
    public class PluginManager {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private List<IHtcPlugin> _plugins;
        private readonly string _pluginsPath;

        public PluginManager(string path) {
            _pluginsPath = path;
            _plugins = new List<IHtcPlugin>();
        }

        public IHtcPlugin[] GetPlugins => _plugins.ToArray();

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

        public void ConstructPlugins() {
            if(!Directory.Exists(_pluginsPath)) {
                Logger.Fatal($"Plugins path '{_pluginsPath}' does not exist!");
                Environment.Exit(2);
            }
            var loaders = (from pluginFile in IoUtils.GetFilesExceptionFix(_pluginsPath, "*.plugin.dll", System.IO.SearchOption.AllDirectories) where File.Exists(pluginFile) select PluginLoader.CreateFromAssemblyFile(pluginFile, sharedTypes: new[] {typeof(IHtcPlugin), typeof(IHttpEvents), typeof(Models.Http.HtcHttpContext)})).ToList();
            _plugins = new List<IHtcPlugin>();
            foreach (var loader in loaders) {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IHtcPlugin).IsAssignableFrom(t) && !t.IsAbstract)) {
                    var plugin = (IHtcPlugin)Activator.CreateInstance(pluginType);
                    _plugins.Add(plugin);
                }
            }
        }
    }
}
