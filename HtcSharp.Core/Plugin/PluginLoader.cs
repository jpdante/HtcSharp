using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal.AssemblyLoader;
using HtcSharp.Logging;

namespace HtcSharp.Core.Plugin {
    public class PluginLoader : ManagedLoadContext, IDisposable {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string AssemblyPath { get; }
        public Assembly? Assembly { get; private set; }
        public List<IPlugin> Instances { get; }

        public PluginLoader(string assemblyPath) : base(assemblyPath, true) {
            AssemblyPath = assemblyPath;
            Instances = new List<IPlugin>();
            Assembly = null;
        }

        public void Load(IVersion version) {
            Assembly = LoadAssemblyFromFilePath(AssemblyPath);
            foreach (var pluginType in Assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)) {
                var plugin = Activator.CreateInstance(pluginType) as IPlugin;
                if (plugin == null) continue;
                if (!plugin.IsCompatible(version)) continue;
                Instances.Add(plugin);
            }
        }

        public void UnloadPlugin(IPlugin plugin) {
            if (!Instances.Contains(plugin)) throw new Exception("This plugin does not belongs to this plugin loader.");
            string pluginName = $"{plugin.Name} v{plugin.Version}";
            try {
                plugin.Dispose();
            } catch (Exception ex) {
                Logger.LogError($"Failed to unload plugin {pluginName}.", ex);
            }
        }

        public void UnloadPlugins() {
            foreach (var instance in Instances) {
                string pluginName = $"{instance.Name} v{instance.Version}";
                try {
                    instance.Dispose();
                } catch (Exception ex) {
                    Logger.LogError($"Failed to unload plugin {pluginName}.", ex);
                }
            }
            Instances.Clear();
        }

        public void Dispose() {
            Unload();
        }
    }
}