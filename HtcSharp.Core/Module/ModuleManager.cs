using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal;
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

        internal bool TryGetBaseModule(IModule module, out BaseModule? baseModule) => _modulesDictionary.TryGetValue(module, out baseModule);

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

        #region Find Modules

        public async Task FindModules(string path) {
            if (!Directory.Exists(path)) return;
            foreach (string assemblyPath in GetFiles(path, "*.module.dll", SearchOption.AllDirectories)) {
                try {
                    await LoadModuleAssemblies(assemblyPath);
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load module assembly '{assemblyPath}'.", ex);
                }
            }
            foreach (var module in Modules) {
                var loaderContext = _modulesDictionary[module].AssemblyLoadContext;
                foreach (var subModule in Modules) {
                    if (subModule == module) continue;
                    var subModuleAssembly = _modulesDictionary[subModule].Assembly;
                    if (string.IsNullOrEmpty(subModuleAssembly.FullName)) continue;
                    loaderContext.AddSharedAssembly(subModuleAssembly.FullName, subModuleAssembly);
                }
            }
        }

        #endregion

        #region Load Assemblies

        public Task LoadModuleAssemblies(string assemblyPath) {
            var assemblyLoadContext = new CustomAssemblyLoadContext(assemblyPath);
            var assembly = assemblyLoadContext.LoadAssemblyFromFilePath(assemblyPath);
            foreach (var moduleType in assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract)) {
                var module = Activator.CreateInstance(moduleType) as IModule;
                if (module == null) continue;
                // TODO: Check if is compatible
                _modules.Add(module);
                _modulesDictionary.Add(module, new BaseModule(module, assembly, assemblyLoadContext));
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Unload Modules

        public async Task UnloadModules() {
            foreach (var module in _modules.ToArray()) {
                try {
                    await UnloadModule(module);
                    Logger.LogInfo($"Unloaded module assembly {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to unload module assembly {module.Name} {module.Version}.", ex);
                }
            }
        }

        public Task UnloadModule(IModule module) {
            if (!_modulesDictionary.TryGetValue(module, out var baseModule)) return Task.CompletedTask;
            baseModule.Unload();
            _modules.Remove(module);
            _modulesDictionary.Remove(module);
            return Task.CompletedTask;
        }

        #endregion

        #region Load Modules

        public async Task LoadModules() {
            foreach (var module in _modules.ToArray()) {
                try {
                    await LoadModule(module);
                    Logger.LogInfo($"Loaded module {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load module {module.Name} {module.Version}.", ex);
                }
            }
        }

        public async Task LoadModule(IModule module) {
            await module.Load();
        }

        #endregion

        #region Enable Modules

        public async Task EnableModules() {
            foreach (var module in _modules.ToArray()) {
                try {
                    await EnableModule(module);
                    Logger.LogInfo($"Enabled module {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to enable module {module.Name} {module.Version}.", ex);
                }
            }
        }

        public async Task EnableModule(IModule module) {
            await module.Enable();
        }

        #endregion

        #region Disable Modules

        public async Task DisableModules() {
            foreach (var module in _modules.ToArray()) {
                try {
                    await DisableModule(module);
                    Logger.LogInfo($"Disabled module {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to disable module {module.Name} {module.Version}.", ex);
                }
            }
        }

        public async Task DisableModule(IModule module) {
            await module.Disable();
        }

        #endregion

    }
}