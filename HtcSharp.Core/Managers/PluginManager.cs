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
        private static readonly Type[] GlobalSharedTypes = new[] {typeof(IHtcPlugin), typeof(IHttpEvents), typeof(Models.Http.HtcHttpContext)};
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

        public void LoadPlugins(string pluginsPath) {
            if(!Directory.Exists(pluginsPath)) {
                Logger.Fatal($"Plugins path '{pluginsPath}' does not exist!");
                Environment.Exit(2);
            }
            var loaders = (from pluginFile in IoUtils.GetFilesExceptionFix(pluginsPath, "*.plugin.dll", SearchOption.AllDirectories) 
                where File.Exists(pluginFile)
                select PluginLoader.CreateFromAssemblyFile(pluginFile, sharedTypes:GlobalSharedTypes)).ToList();
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

        public List<IHtcPlugin> LoadPluginsCustom(string pluginsPath, string searchPattern, Type[] sharedTypes) {
            if(!Directory.Exists(pluginsPath)) {
                Logger.Fatal($"Plugins path '{pluginsPath}' does not exist!");
                Environment.Exit(2);
            }
            var types = new Type[GlobalSharedTypes.Length + sharedTypes.Length];
            GlobalSharedTypes.CopyTo(types, 0);
            sharedTypes.CopyTo(types, GlobalSharedTypes.Length);
            var loaders = (from pluginFile in IoUtils.GetFilesExceptionFix(pluginsPath, searchPattern, SearchOption.AllDirectories) 
                where File.Exists(pluginFile)
                select PluginLoader.CreateFromAssemblyFile(pluginFile, sharedTypes: types)).ToList();
            var plugins = new List<IHtcPlugin>();
            foreach (var loader in loaders) {
                plugins.AddRange(loader.LoadDefaultAssembly().GetTypes().Where(t => typeof(IHtcPlugin).IsAssignableFrom(t) && !t.IsAbstract).Select(pluginType => (IHtcPlugin) Activator.CreateInstance(pluginType)));
            }
            return plugins;
        }

        public IHtcPlugin[] GetPlugins => _plugins.ToArray();
    }
}
