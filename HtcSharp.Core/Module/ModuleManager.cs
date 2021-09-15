using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Abstractions.Manager;
using HtcSharp.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace HtcSharp.Core.Module {
    public class ModuleManager : IModuleManager {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IVersion _version;
        private readonly List<IModule> _modules;
        private readonly Dictionary<IModule, ModuleLoader> _modulesDictionary;
        private readonly IServiceProvider _serviceProvider;

        public IEnumerable<IReadOnlyModule> Modules => _modules;

        public ModuleManager(IVersion version, Action<IServiceCollection> configureServices) {
            _version = version;
            _modules = new List<IModule>();
            _modulesDictionary = new Dictionary<IModule, ModuleLoader>();
            var serviceCollection = new ServiceCollection();
            configureServices.Invoke(serviceCollection);
            serviceCollection.AddSingleton<IModuleManager>(this);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public bool TryGetBaseModule(IReadOnlyModule module, out ModuleLoader? baseModule) => _modulesDictionary.TryGetValue((IModule) module, out baseModule);

        public bool HasModule(string name) => _modules.Any(module => module.Name == name);

        #region Load Modules

        public async Task LoadModules(string path) {
            if (!Directory.Exists(path)) return;
            foreach (string assemblyPath in GetFiles(path, "*.module.dll", SearchOption.AllDirectories)) {
                try {
                    await LoadModule(assemblyPath);
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load module assembly '{assemblyPath}'.", ex);
                }
            }
        }

        public Task LoadModule(string assemblyPath) {
            var moduleLoader = new ModuleLoader(assemblyPath);
            moduleLoader.Load(_version);
            if (moduleLoader.Instances.Count == 0) {
                moduleLoader.Dispose();
                return Task.CompletedTask;
            }
            foreach (var instance in moduleLoader.Instances) {
                _modulesDictionary.Add(instance, moduleLoader);
                _modules.Add(instance);
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
            if (!_modulesDictionary.TryGetValue(module, out var moduleLoader)) return Task.CompletedTask;
            moduleLoader.UnloadModule(module);
            _modules.Remove(module);
            _modulesDictionary.Remove(module);
            if (moduleLoader.Instances.Count == 0) {
                moduleLoader.Dispose();
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Init Modules

        public async Task InitModules() {
            foreach (var module in _modules) {
                try {
                    await InitModule(module);
                    Logger.LogInfo($"Initialized module {module.Name} {module.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to initialize module {module.Name} {module.Version}.", ex);
                }
            }
        }

        public async Task InitModule(IModule module) {
            await module.Init(_serviceProvider);
        }

        #endregion

        #region Enable Modules

        public async Task EnableModules() {
            foreach (var module in _modules) {
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
            foreach (var module in _modules) {
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

        private static IEnumerable<string> GetFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            List<string> files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly).ToList();
            if (searchOption == SearchOption.TopDirectoryOnly) return files.ToArray();
            foreach (string subDir in Directory.GetDirectories(path)) {
                try {
                    files.AddRange(GetFiles(subDir, searchPattern, searchOption));
                } catch {
                    // ignored
                }
            }
            return files;
        }
    }
}