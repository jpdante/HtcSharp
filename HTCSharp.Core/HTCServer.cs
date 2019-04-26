using HTCSharp.Core.Engines;
using HTCSharp.Core.Helpers.Http;
using HTCSharp.Core.IO;
using HTCSharp.Core.Logging;
using HTCSharp.Core.Managers;
using HTCSharp.Core.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace HTCSharp.Core {
    public class HTCServer {
        private static readonly ILog Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, Type> _availableEngines;
        private List<Engine> _engines;
        private PluginManager _pluginsManager;
        private JObject _config;
        private readonly string _configPath;
        private string _pluginsPath;

        public static bool IsDebug { get; private set; }
        public static HTCServer Context { get; private set; }

        public T GetConfig<T>(string key) { 
            return _config.GetValue(key, StringComparison.CurrentCultureIgnoreCase).Value<T>();
        }

        public string AspNetConfigPath { get; private set; }
        public bool IsStopped { get; private set; }
        public ReadOnlyCollection<Engine> GetEngines() { return _engines.AsReadOnly(); }

        public HTCServer(string configPath) {
            Context = this;
            _configPath = configPath;
            IsStopped = false;
            IsDebug = false;
            _engines = new List<Engine>();
        }

        public void Start() {
            _availableEngines = new Dictionary<string, Type>();
            URLMapping.RegisterIndexFile("index.html");
            URLMapping.RegisterIndexFile("index.htm");
            RegisterEngine("http", typeof(HttpEngine));
            _engines = new List<Engine>();
            Logger.Info("Starting HTCSharp...");
            Logger.Info("Loading Configuration...");
            AspNetConfigPath = Path.Combine(Path.GetDirectoryName(_configPath), "aspnet-appsettings.json");
            try {
                if (!File.Exists(_configPath)) IOUtils.CreateHtcConfig(_configPath);
                _config = IOUtils.GetJsonFile(_configPath);
                if(!File.Exists(AspNetConfigPath)) IOUtils.CreateAspConfig(AspNetConfigPath);
                _pluginsPath = _config.GetValue("PluginsPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>() ?? Path.Combine(Directory.GetCurrentDirectory(), @"plugins/");
                _pluginsPath = IOUtils.ReplacePathTags(_pluginsPath);
                IsDebug = _config.GetValue("Debug", StringComparison.CurrentCultureIgnoreCase)?.Value<bool>() == true;
                if (!Directory.Exists(_pluginsPath)) Directory.CreateDirectory(_pluginsPath);
            } catch (Exception ex) {
                Logger.Error("Failed to load configuration!", ex);
                return;
            }
            _pluginsManager = new PluginManager(_pluginsPath);
            _pluginsManager.ConstructPlugins();
            _pluginsManager.Call_OnLoad();
            LoadEngines();
            foreach(var engine in _engines) {
                try {
                    Logger.Info($"Starting Engine: '{engine.GetType().Name}'");
                    engine.Start();
                } catch(Exception ex) {
                    Logger.Error($"Failed to start Engine: '{engine.GetType().Name}'", ex);
                }
            }
            IsStopped = true;
            _pluginsManager.Call_OnEnable();
            Logger.Info("HTCServer is now running!");
        }

        public void Stop() {
            Logger.Info("Shutting down HTCSharp...");
            _pluginsManager.Call_OnDisable();
            IsStopped = false;
            foreach (var engine in _engines) {
                try {
                    Logger.Info($"Stopping Engine: '{engine.GetType().Name}'");
                    engine.Stop();
                } catch (Exception ex) {
                    Logger.Error($"Failed to stop Engine: '{engine.GetType().Name}'", ex);
                }
            }
            this._engines.Clear();
            this._availableEngines.Clear();
        }

        public void WaitStop(bool daemon) {
            if(daemon) {
                while (IsStopped) {
                    System.Threading.Thread.Sleep(100);
                }
            } else {
                while (IsStopped) {
                    var line = Console.ReadLine();
                    if (line == null) continue;
                    var args = line.Split(' ');
                    switch (args[0]) {
                        case "stop":
                            Stop();
                            break;
                        case "close":
                            Stop();
                            break;
                        case "exit":
                            Stop();
                            break;
                        default:
                            Logger.Info("This command does not exist!");
                            break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public void RegisterEngine(string engineName, Type type) {
            if (!_availableEngines.ContainsKey(engineName.ToLower())) _availableEngines.Add(engineName, type);
            else {
                throw new Exceptions.Dictionary.KeyAlreadyExistException(engineName.ToLower());
            }
        }

        public void UnregisterEngine(string engineName) {
            if(_availableEngines.ContainsKey(engineName.ToLower())) _availableEngines.Remove(engineName);
            else {
                throw new Exceptions.Dictionary.KeyNotFoundException(engineName.ToLower());
            }
        }

        private void LoadEngines() {
            var servers = _config.GetValue("Engines", StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if(servers == null) return;
            foreach(var jToken in servers) {
                var server = (JObject) jToken;
                var engineName = server.GetValue("Engine", StringComparison.CurrentCultureIgnoreCase)?.Value<string>();
                var engineConfig = server.GetValue("Config", StringComparison.CurrentCultureIgnoreCase)?.Value<JObject>();
                if(engineName == null) continue;
                if(_availableEngines.ContainsKey(engineName.ToLower())) {
                    var type = _availableEngines[engineName.ToLower()];
                    Logger.Info($"Creating Engine: '{type.Name}'");
                    var instance = (Engine)Activator.CreateInstance(type);
                    instance.SetConfig(engineConfig);
                    _engines.Add(instance);
                } else {
                    Logger.Info($"Unable to create, Engine '{engineName}' does not exist.");
                }
            }
        }
    }
}
