using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal.AssemblyLoader;
using HtcSharp.Logging;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace HtcSharp.Core.Module {
    public class ModuleLoader : IDisposable {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public readonly Dictionary<string, Assembly> SharedAssemblies;
        public readonly Dictionary<string, Assembly> LoadedAssemblies;
        public readonly List<string> PrivateAssemblies;
        public readonly List<string> AdditionalProbingPaths;
        public readonly List<string> ResourceProbingPaths;

        public bool ShadowCopyNativeLibraries { get; set; }

        public string AssemblyPath { get; }
        public Assembly? Assembly { get; private set; }
        public List<IModule> Instances { get; }
        public ManagedLoadContext AssemblyLoadContext { get; }

        public ModuleLoader(string assemblyPath) {
            AssemblyPath = assemblyPath;
            Assembly = null;
            Instances = new List<IModule>();

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
            foreach (var moduleType in Assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract)) {
                var module = Activator.CreateInstance(moduleType) as IModule;
                if (module == null) continue;
                if (!module.IsCompatible(version)) continue;
                Instances.Add(module);
            }
        }

        public void UnloadModule(IModule module) {
            if (!Instances.Contains(module)) throw new Exception("This module does not belongs to this module loader.");
            string moduleName = $"{module.Name} v{module.Version}";
            try {
                module.Dispose();
            } catch (Exception ex) {
                Logger.LogError($"Failed to unload module {moduleName}.", ex);
            }
        }

        public void UnloadAll() {
            foreach (var instance in Instances) {
                string moduleName = $"{instance.Name} v{instance.Version}";
                try {
                    instance.Dispose();
                } catch (Exception ex) {
                    Logger.LogError($"Failed to unload module {moduleName}.", ex);
                }
            }

            Instances.Clear();
        }

        public void Dispose() {
            AssemblyLoadContext.Unload();
        }
    }
}