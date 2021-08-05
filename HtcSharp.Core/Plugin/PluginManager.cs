using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Core.Module;
using HtcSharp.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace HtcSharp.Core.Plugin {
    public class PluginManager : IPluginManager {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IVersion _version;
        private readonly List<IPlugin> _plugins;
        private readonly Dictionary<IPlugin, PluginLoader> _pluginsDictionary;
        private readonly ModuleManager _moduleManager;
        private readonly IServiceProvider _serviceProvider;

        public IEnumerable<IReadOnlyPlugin> Plugins => _plugins;

        public bool HasPlugin(string name) => _plugins.Any(module => module.Name == name);

        public PluginManager(IVersion version, ModuleManager moduleManager, Action<IServiceCollection> configureServices) {
            _version = version;
            _moduleManager = moduleManager;
            _plugins = new List<IPlugin>();
            _pluginsDictionary = new Dictionary<IPlugin, PluginLoader>();
            var serviceCollection = new ServiceCollection();
            configureServices.Invoke(serviceCollection);
            serviceCollection.AddSingleton<IModuleManager>(_moduleManager);
            serviceCollection.AddSingleton<IPluginManager>(this);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        internal bool TryGetBasePlugin(IReadOnlyPlugin plugin, out PluginLoader? basePlugin) => _pluginsDictionary.TryGetValue((IPlugin) plugin, out basePlugin);

        #region Load Plugins

        public async Task LoadPlugins(string path) {
            if (!Directory.Exists(path)) return;
            foreach (string assemblyPath in GetFiles(path, "*.plugin.dll", SearchOption.AllDirectories)) {
                try {
                    await LoadPlugin(assemblyPath);
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load plugin '{assemblyPath}'.", ex);
                }
            }
        }

        public Task LoadPlugin(string assemblyPath) {
            var pluginLoader = new PluginLoader(assemblyPath);

            // Add modules as shared libraries
            foreach (var readOnlyModule in _moduleManager.Modules) {
                if (!_moduleManager.TryGetBaseModule(readOnlyModule, out var moduleLoader)) continue;
                if (moduleLoader == null) continue;
                if (moduleLoader.Assembly == null) continue;
                string? moduleAssemblyName = moduleLoader.Assembly.GetName().Name;
                if (string.IsNullOrEmpty(moduleAssemblyName)) continue;
                pluginLoader.SharedAssemblies.Add(moduleAssemblyName, moduleLoader.Assembly);   
                pluginLoader.SharedContexts.Add(moduleLoader);
            }

            // Add plugins as shared libraries
            foreach (var subPluginLoader in _pluginsDictionary.Values) {
                pluginLoader.SharedContexts.Add(subPluginLoader);
            }

            // Load plugins
            pluginLoader.Load(_version);
            if (pluginLoader.Instances.Count == 0) {
                pluginLoader.Dispose();
                return Task.CompletedTask;
            }
            foreach (var instance in pluginLoader.Instances) {
                _pluginsDictionary.Add(instance, pluginLoader);
                _plugins.Add(instance);
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Unload Plugins

        public async Task UnloadPlugins() {
            foreach (var plugin in _plugins) {
                try {
                    await UnloadPlugin(plugin);
                    Logger.LogInfo($"Unloaded plugin assembly {plugin.Name} {plugin.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to load plugin {plugin.Name} {plugin.Version}.", ex);
                }
            }
        }

        public Task UnloadPlugin(IPlugin plugin) {
            if (!_pluginsDictionary.TryGetValue(plugin, out var pluginLoader)) return Task.CompletedTask;
            pluginLoader.UnloadPlugin(plugin);
            _plugins.Remove(plugin);
            _pluginsDictionary.Remove(plugin);
            if (pluginLoader.Instances.Count == 0) {
                pluginLoader.Dispose();
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Init Plugins

        public async Task InitPlugins() {
            foreach (var plugin in _plugins.ToArray()) {
                try {
                    await InitPlugin(plugin);
                    Logger.LogInfo($"Initialized plugin {plugin.Name} {plugin.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to initialize plugin {plugin.Name} {plugin.Version}.", ex);
                }
            }
        }

        public async Task InitPlugin(IPlugin plugin) {
            await plugin.Init(_serviceProvider);
        }

        #endregion

        #region Enable Plugins

        public async Task EnablePlugins() {
            foreach (var plugin in _plugins.ToArray()) {
                try {
                    await EnablePlugin(plugin);
                    Logger.LogInfo($"Enabled plugin {plugin.Name} {plugin.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to enable plugin {plugin.Name} {plugin.Version}.", ex);
                }
            }
        }

        public async Task EnablePlugin(IPlugin plugin) {
            await plugin.Enable();
        }

        #endregion

        #region Disable Plugins

        public async Task DisablePlugins() {
            foreach (var plugin in _plugins.ToArray()) {
                try {
                    await DisablePlugin(plugin);
                    Logger.LogInfo($"Disabled plugin {plugin.Name} {plugin.Version}.");
                } catch (Exception ex) {
                    Logger.LogError($"Failed to disable plugin {plugin.Name} {plugin.Version}.", ex);
                }
            }
        }

        public async Task DisablePlugin(IPlugin plugin) {
            await plugin.Disable();
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
            return files.ToArray();
        }
    }
}