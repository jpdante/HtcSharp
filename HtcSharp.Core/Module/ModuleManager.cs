using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using HtcSharp.Abstractions;
using HtcSharp.Logging;

namespace HtcSharp.Core.Module {
    public class ModuleManager {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly List<IModule> _modules;
        private readonly AssemblyLoadContext _assemblyLoadContext;

        public IReadOnlyList<IModule> Modules => _modules;

        public ModuleManager() {
            _modules = new List<IModule>();
            _assemblyLoadContext = new AssemblyLoadContext("ModuleContext", true);
        }

        public void LoadModules(string path) {
            foreach (string assemblyPath in GetFiles(path, "*.module.dll", SearchOption.AllDirectories)) {
                LoadModule(assemblyPath);
            }
        }

        public void LoadModule(string assemblyPath) {
            try {
                var assembly = _assemblyLoadContext.LoadFromAssemblyPath(assemblyPath);
                foreach (var moduleType in assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract)) {
                    var module = Activator.CreateInstance(moduleType) as IModule;
                    if (module == null) continue;
                    _modules.Add(module);
                    Logger.LogInfo($"Loaded module {module.Name} {module.Version}.");
                }
            } catch (Exception ex) {
                Logger.LogError($"Failed to load module '{assemblyPath}'.", ex);
            }
        }

        public void UnloadModules() {

        }

        public void UnloadModule(IModule module) {

        }

        public static string[] GetFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            List<string> files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly).ToList();
            if (searchOption == SearchOption.TopDirectoryOnly) return files.ToArray();
            foreach (string subDir in Directory.GetDirectories(path)) {
                try {
                    files.AddRange(GetFiles(subDir, searchPattern, searchOption));
                } catch {
                    // ignored
                }
            }
            return files.ToArray();
        }

    }
}