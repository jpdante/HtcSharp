using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Logging;

namespace HtcSharp.Core.Module {
    public class ModuleManager {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly List<IModule> _modules;
        private readonly Dictionary<IModule, BaseModule> _modulesDictionary;

        public IReadOnlyList<IModule> Modules => _modules;

        public ModuleManager() {
            _modules = new List<IModule>();
            _modulesDictionary = new Dictionary<IModule, BaseModule>();
        }

        public async Task LoadModules(string path) {
            foreach (string assemblyPath in GetFiles(path, "*.module.dll", SearchOption.AllDirectories)) {
                try {
                    await LoadModule(assemblyPath);
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load module '{assemblyPath}'.", ex);
                }
            }
        }

        public async Task LoadModule(string assemblyPath) {
            var assemblyLoadContext = new AssemblyLoadContext("ModuleContext", true);
            var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblyPath);
            foreach (var moduleType in assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract)) {
                var module = Activator.CreateInstance(moduleType) as IModule;
                if (module == null) continue;
                // TODO: Check if is compatible
                _modules.Add(module);
                _modulesDictionary.Add(module, new BaseModule(module, assemblyLoadContext));
                await module.Load();
                Logger.LogInfo($"Loaded module {module.Name} {module.Version}.");
            }
        }

        public async Task InitModules() {
            foreach (var module in _modules.ToArray()) {
                try {
                    await InitModule(module);
                    Logger.LogInfo($"Enabled module {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to enable module {module.Name} {module.Version}.", ex);
                }
            }
        }

        public async Task InitModule(IModule module) {
            await module.Enable();
        }

        public async Task UnloadModules() {
            foreach (var module in _modules.ToArray()) {
                try {
                    await UnloadModule(module);
                    Logger.LogInfo($"Unloaded module {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load module {module.Name} {module.Version}.", ex);
                }
            }
        }

        public async Task UnloadModule(IModule module) {
            if (!_modulesDictionary.TryGetValue(module, out var baseModule)) return;
            await module.Disable();
            baseModule.Unload();
            _modules.Remove(module);
            _modulesDictionary.Remove(module);
        }

        private static string[] GetFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
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