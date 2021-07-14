using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace HtcSharp.Core.Internal {
    internal class CustomAssemblyLoadContext : AssemblyLoadContext {
        private readonly AssemblyDependencyResolver _dependencyResolver;

        public CustomAssemblyLoadContext(string mainAssemblyPath) : base(mainAssemblyPath, true) {
            _dependencyResolver = new AssemblyDependencyResolver(mainAssemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName) {
            if (string.IsNullOrEmpty(assemblyName.Name)) throw new ArgumentNullException(nameof(assemblyName));

            string? resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath)) {
                return LoadAssemblyFromFilePath(resolvedPath);
            }

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