using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using HtcSharp.Logging;

namespace HtcSharp.Core.Internal {
    internal class CustomAssemblyLoadContext : AssemblyLoadContext {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly AssemblyDependencyResolver _dependencyResolver;
        private readonly Dictionary<string, Assembly> _sharedAssemblies;
        private readonly Dictionary<string, Assembly> _loadedAssemblies;

        public IReadOnlyDictionary<string, Assembly> SharedAssemblies => _sharedAssemblies;
        public IReadOnlyDictionary<string, Assembly> LoadedAssemblies => _loadedAssemblies;

        public CustomAssemblyLoadContext(string mainAssemblyPath) : base(mainAssemblyPath, true) {
            _dependencyResolver = new AssemblyDependencyResolver(mainAssemblyPath);
            _sharedAssemblies = new Dictionary<string, Assembly>();
            _loadedAssemblies = new Dictionary<string, Assembly>();
        }

        public void AddSharedAssembly(string assemblyName, Assembly assembly) {
            _sharedAssemblies.Add(assemblyName, assembly);
        }

        protected override Assembly? Load(AssemblyName assemblyName) {
            if (string.IsNullOrEmpty(assemblyName.Name)) throw new ArgumentNullException(nameof(assemblyName));

            if (_sharedAssemblies.TryGetValue(assemblyName.FullName, out var assembly)) {
                //Logger.LogDebug($"Loading from Shared Assembly: {assemblyName.FullName}");
                return assembly;
            }

            string? resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath)) {
                var loadedAssembly = LoadAssemblyFromFilePath(resolvedPath);
                if (loadedAssembly != null && !string.IsNullOrEmpty(loadedAssembly.FullName)) _loadedAssemblies.Add(loadedAssembly.FullName, loadedAssembly);
                return loadedAssembly;
            }

            //Logger.LogDebug($"Failed to load assembly: {assemblyName.FullName}");

            return null;
        }

        public Assembly LoadAssemblyFromFilePath(string path) {
            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            string pdbPath = Path.ChangeExtension(path, ".pdb");
            if (!File.Exists(pdbPath)) return LoadFromStream(file);
            using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return LoadFromStream(file, pdbFile);
        }
    }
}