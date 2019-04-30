using HTCSharp.Core.Interfaces.Plugin;
using HTCSharp.Core.IO;
using HTCSharp.Core.Logging;
using HTCSharp.Core.Utils;
using McMaster.NETCore.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HTCSharp.Core.Managers {
    public class PluginManager {
        private static readonly ILog _Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private List<IHTCPlugin> Plugins;
        private string PluginsPath;

        public PluginManager(string path) {
            PluginsPath = path;
            Plugins = new List<IHTCPlugin>();
        }

        public IHTCPlugin[] GetPlugins { get { return Plugins.ToArray(); } }

        public void Call_OnLoad() {
            foreach (IHTCPlugin plugin in Plugins) {
                _Logger.Info($"Loading: {plugin.PluginName}({plugin.PluginVersion})");
                plugin.OnLoad();
            }
        }

        public void Call_OnEnable() {
            foreach (IHTCPlugin plugin in Plugins) {
                plugin.OnEnable();
                _Logger.Info($"Enabled: {plugin.PluginName}({plugin.PluginVersion})");
            }
        }

        public void Call_OnDisable() {
            foreach (IHTCPlugin plugin in Plugins) {
                plugin.OnDisable();
                _Logger.Info($"Disabled: {plugin.PluginName}({plugin.PluginVersion})");
            }
        }

        public void ConstructPlugins() {
            if(!Directory.Exists(PluginsPath)) {
                _Logger.Fatal($"Plugins path '{PluginsPath}' does not exist!");
                Environment.Exit(2);
            }
            var loaders = (from pluginFile in IOUtils.GetFilesExceptionFix(PluginsPath, "*.plugin.dll", System.IO.SearchOption.AllDirectories) where File.Exists(pluginFile) select PluginLoader.CreateFromAssemblyFile(pluginFile, sharedTypes: new[] {typeof(IHTCPlugin), typeof(IHttpEvents), typeof(Models.Http.HTCHttpContext)})).ToList();
            Plugins = new List<IHTCPlugin>();
            foreach (var loader in loaders) {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IHTCPlugin).IsAssignableFrom(t) && !t.IsAbstract)) {
                    var plugin = (IHTCPlugin)Activator.CreateInstance(pluginType);
                    Plugins.Add(plugin);
                }
            }
        }
    }
}
