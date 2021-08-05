// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using HtcSharp.Logging;

namespace HtcSharp.Core.Internal.AssemblyLoader {
    public class ManagedLoadContext : AssemblyLoadContext {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly string _basePath;
        private readonly string _mainAssemblyPath;
        private readonly AssemblyDependencyResolver _dependencyResolver;
        private readonly bool _shadowCopyNativeLibraries;

        public readonly Dictionary<string, Assembly> SharedAssemblies;
        public readonly Dictionary<string, Assembly> LoadedAssemblies;
        public readonly List<ManagedLoadContext> SharedContexts;
        public readonly List<string> PrivateAssemblies;
        public readonly List<string> AdditionalProbingPaths;
        public readonly List<string> ResourceProbingPaths;

        private readonly string[] _resourceRoots;
        private readonly string _unmanagedDllShadowCopyDirectoryPath;

        public ManagedLoadContext(string mainAssemblyPath, bool shadowCopyNativeLibraries) : base(mainAssemblyPath, true) {
            _mainAssemblyPath = mainAssemblyPath ?? throw new ArgumentNullException(nameof(mainAssemblyPath));
            _dependencyResolver = new AssemblyDependencyResolver(mainAssemblyPath);
            _basePath = Path.GetDirectoryName(mainAssemblyPath) ?? throw new ArgumentException(nameof(mainAssemblyPath));

            SharedAssemblies = new Dictionary<string, Assembly>();
            LoadedAssemblies = new Dictionary<string, Assembly>();
            SharedContexts = new List<ManagedLoadContext>();
            PrivateAssemblies = new List<string>();
            AdditionalProbingPaths = new List<string>();
            ResourceProbingPaths = new List<string>();
            _shadowCopyNativeLibraries = shadowCopyNativeLibraries;

            _resourceRoots = new[] { _basePath }.Concat(ResourceProbingPaths).ToArray();
            _unmanagedDllShadowCopyDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (_shadowCopyNativeLibraries) {
                Unloading += _ => OnUnloaded();
            }
        }

        protected override Assembly? Load(AssemblyName assemblyName) {
            if (string.IsNullOrEmpty(assemblyName.Name)) throw new ArgumentNullException(nameof(assemblyName));
            // Ignore Resolver
            if (PrivateAssemblies.Contains(assemblyName.Name)) return null;

            Assembly? assembly;

            // Native Resolver
            string? resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath)) {
                assembly = LoadAssemblyFromFilePath(resolvedPath);
                LoadedAssemblies.Add(assemblyName.Name, assembly);
                return assembly;
            }

            // Probe Resource Roots
            if (!string.IsNullOrEmpty(assemblyName.CultureName) && !string.Equals("neutral", assemblyName.CultureName)) {
                foreach (var resourceRoot in _resourceRoots) {
                    string resourcePath = Path.Combine(resourceRoot, assemblyName.CultureName, assemblyName.Name + ".dll");
                    if (!File.Exists(resourcePath)) continue;
                    assembly = LoadAssemblyFromFilePath(resourcePath);
                    LoadedAssemblies.Add(assemblyName.Name, assembly);
                    return assembly;
                }

                return null;
            }

            // Shared Resolver
            if (SharedAssemblies.TryGetValue(assemblyName.Name, out assembly) && assembly != null) {
                LoadedAssemblies.Add(assemblyName.Name, assembly);
                return assembly;
            } else {
                string dllName = assemblyName.Name + ".dll";
                foreach (var probingPath in AdditionalProbingPaths.Prepend(_basePath)) {
                    string localFile = Path.Combine(probingPath, dllName);
                    if (!File.Exists(localFile)) continue;
                    assembly = LoadAssemblyFromFilePath(localFile);
                    LoadedAssemblies.Add(assemblyName.Name, assembly);
                    return assembly;
                }
            }

            foreach (var sharedContext in SharedContexts) {
                if (!sharedContext.LoadedAssemblies.TryGetValue(assemblyName.Name, out assembly)) continue;
                LoadedAssemblies.Add(assemblyName.Name, assembly);
                return assembly;
            }
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
            string? resolvedPath = _dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath)) {
                return LoadUnmanagedDllFromResolvedPath(resolvedPath, normalizePath: false);
            }

            foreach (var prefix in PlatformInformation.NativeLibraryPrefixes) {
                foreach (var suffix in PlatformInformation.NativeLibraryExtensions) {
                    if (!unmanagedDllName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }

                    // check to see if there is a library entry for the library without the file extension
                    string trimmedName = unmanagedDllName.Substring(0, unmanagedDllName.Length - suffix.Length);

                    // fallback to native assets which match the file name in the plugin base directory
                    string prefixSuffixDllName = prefix + unmanagedDllName + suffix;
                    string prefixDllName = prefix + unmanagedDllName;

                    foreach (var probingPath in AdditionalProbingPaths.Prepend(_basePath)) {
                        string localFile = Path.Combine(probingPath, prefixSuffixDllName);
                        if (File.Exists(localFile)) {
                            return LoadUnmanagedDllFromResolvedPath(localFile);
                        }

                        string localFileWithoutSuffix = Path.Combine(probingPath, prefixDllName);
                        if (File.Exists(localFileWithoutSuffix)) {
                            return LoadUnmanagedDllFromResolvedPath(localFileWithoutSuffix);
                        }
                    }
                }
            }

            return base.LoadUnmanagedDll(unmanagedDllName);
        }

        public Assembly LoadAssemblyFromFilePath(string path) {
            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            string pdbPath = Path.ChangeExtension(path, ".pdb");

            if (!File.Exists(pdbPath)) return LoadFromStream(file);
            using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return LoadFromStream(file, pdbFile);
        }

        private IntPtr LoadUnmanagedDllFromResolvedPath(string unmanagedDllPath, bool normalizePath = true) {
            if (normalizePath) unmanagedDllPath = Path.GetFullPath(unmanagedDllPath);

            return _shadowCopyNativeLibraries
                ? LoadUnmanagedDllFromShadowCopy(unmanagedDllPath)
                : LoadUnmanagedDllFromPath(unmanagedDllPath);
        }

        private IntPtr LoadUnmanagedDllFromShadowCopy(string unmanagedDllPath) {
            string shadowCopyDllPath = CreateShadowCopy(unmanagedDllPath);
            return LoadUnmanagedDllFromPath(shadowCopyDllPath);
        }

        private string CreateShadowCopy(string dllPath) {
            Directory.CreateDirectory(_unmanagedDllShadowCopyDirectoryPath);

            string dllFileName = Path.GetFileName(dllPath);
            string shadowCopyPath = Path.Combine(_unmanagedDllShadowCopyDirectoryPath, dllFileName);

            if (!File.Exists(shadowCopyPath)) {
                File.Copy(dllPath, shadowCopyPath);
            }

            return shadowCopyPath;
        }

        private void OnUnloaded() {
            if (!_shadowCopyNativeLibraries || !Directory.Exists(_unmanagedDllShadowCopyDirectoryPath)) {
                return;
            }

            // Attempt to delete shadow copies
            try {
                Directory.Delete(_unmanagedDllShadowCopyDirectoryPath, recursive: true);
            } catch (Exception) {
                // Files might be locked by host process. Nothing we can do about it, I guess.
            }
        }
    }
}