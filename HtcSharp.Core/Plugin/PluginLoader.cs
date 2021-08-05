using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal.AssemblyLoader;
using HtcSharp.Logging;

namespace HtcSharp.Core.Plugin {
    public class PluginLoader : IDisposable {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public readonly Dictionary<string, Assembly> SharedAssemblies;
        public readonly Dictionary<string, Assembly> LoadedAssemblies;
        public readonly List<string> PrivateAssemblies;
        public readonly List<string> AdditionalProbingPaths;
        public readonly List<string> ResourceProbingPaths;

        public bool ShadowCopyNativeLibraries { get; set; }

        public string AssemblyPath { get; }
        public Assembly? Assembly { get; private set; }
        public List<IPlugin> Instances { get; }
        public ManagedLoadContext AssemblyLoadContext { get; private set; }

        public PluginLoader(string assemblyPath) {
            AssemblyPath = assemblyPath;
            Instances = new List<IPlugin>();
            Assembly = null;

            SharedAssemblies = new Dictionary<string, Assembly>();
            LoadedAssemblies = new Dictionary<string, Assembly>();
            PrivateAssemblies = new List<string>();
            AdditionalProbingPaths = new List<string>();
            ResourceProbingPaths = new List<string>();
            ShadowCopyNativeLibraries = false;

            AssemblyLoadContext = new ManagedLoadContext(AssemblyPath, SharedAssemblies, PrivateAssemblies, AdditionalProbingPaths, ResourceProbingPaths, ShadowCopyNativeLibraries);
            AssemblyLoadContext.LoadManagedLibrary += OnLoadManagedLibrary;
        }

        private void OnLoadManagedLibrary(AssemblyName assemblyName, Assembly assembly) {
            if (assemblyName.Name == null) return;
            LoadedAssemblies.Add(assemblyName.Name, assembly);
        }

        public void Load(IVersion version) {
            Assembly = AssemblyLoadContext.LoadAssemblyFromFilePath(AssemblyPath);
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

        public void UnloadAll() {
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
            AssemblyLoadContext.Unload();
        }
    }
}