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
        private static readonly ILog _Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        private static HTCServer _HtcServer;
        private static bool _Debug;

        private Dictionary<string, Type> AvailableEngines;
        private List<Engine> Engines;
        private PluginManager PluginsManager;
        private JObject Config;
        private bool Active;
        private string AspNetConfig;
        private string ConfigPath;
        private string PluginsPath;

        public static bool IsDebug { get { return _Debug; } }
        public static HTCServer Context { get { return _HtcServer; } }

        public T GetConfig<T>(string key) { 
            return Config.GetValue(key, StringComparison.CurrentCultureIgnoreCase).Value<T>();
        }

        public string AspNetConfigPath { get { return AspNetConfig; } }
        public bool IsStopped { get { return Active; } }
        public ReadOnlyCollection<Engine> GetEngines() { return Engines.AsReadOnly(); }

        public HTCServer(string configPath) {
            _HtcServer = this;
            ConfigPath = configPath;
            Active = false;
            _Debug = false;
        }

        public void Start() {
            AvailableEngines = new Dictionary<string, Type>();
            URLMapping.RegisterIndexFile("index.html");
            URLMapping.RegisterIndexFile("index.htm");
            RegisterEngine("http", typeof(HttpEngine));
            Engines = new List<Engine>();
            _Logger.Info("Starting HTCSharp...");
            _Logger.Info("Loading Configuration...");
            try {
                Config = IOUtils.GetJsonFile(ConfigPath);
                AspNetConfig = Path.Combine(Path.GetDirectoryName(ConfigPath), "aspnet-appsettings.json");
                IOUtils.CreateHttpConfig(AspNetConfig);
                PluginsPath = Config.GetValue("PluginsPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>() ?? Path.Combine(Directory.GetCurrentDirectory(), @"plugins/");
                PluginsPath = IOUtils.ReplacePathTags(PluginsPath);
                _Debug = Config.GetValue("Debug", StringComparison.CurrentCultureIgnoreCase)?.Value<bool>() == true;
            } catch (Exception ex) {
                _Logger.Error("Failed to load configuration!", ex);
                return;
            }
            PluginsManager = new PluginManager(PluginsPath);
            PluginsManager.ConstructPlugins();
            PluginsManager.Call_OnLoad();
            LoadEngines();
            foreach(Engine engine in Engines) {
                try {
                    _Logger.Info($"Starting Engine: '{engine.GetType().Name}'");
                    engine.Start();
                } catch(Exception ex) {
                    _Logger.Error($"Failed to start Engine: '{engine.GetType().Name}'", ex);
                }
            }
            Active = true;
            PluginsManager.Call_OnEnable();
            _Logger.Info("HTCServer is now running!");
        }

        public void Stop() {
            _Logger.Info("Shutting down HTCSharp...");
            PluginsManager.Call_OnDisable();
            Active = false;
            foreach (Engine engine in Engines) {
                try {
                    _Logger.Info($"Stopping Engine: '{engine.GetType().Name}'");
                    engine.Stop();
                } catch (Exception ex) {
                    _Logger.Error($"Failed to stop Engine: '{engine.GetType().Name}'", ex);
                }
            }
            this.Engines.Clear();
            this.AvailableEngines.Clear();
        }

        public void WaitStop(bool daemon) {
            if(daemon) {
                while (Active) {
                    System.Threading.Thread.Sleep(100);
                }
            } else {
                while (Active) {
                    string rawcmd = Console.ReadLine();
                    if (rawcmd == null) continue;
                    string[] cmds = rawcmd.Split(' ');
                    switch (cmds[0]) {
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
                            _Logger.Info("This command does not exist!");
                            break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public void RegisterEngine(string engineName, Type type) {
            if (!AvailableEngines.ContainsKey(engineName.ToLower())) AvailableEngines.Add(engineName, type);
            else {
                throw new Exceptions.Dictionary.KeyAlreadyExistException(engineName.ToLower());
            }
        }

        public void UnregisterEngine(string engineName) {
            if(AvailableEngines.ContainsKey(engineName.ToLower())) AvailableEngines.Remove(engineName);
            else {
                throw new Exceptions.Dictionary.KeyNotFoundException(engineName.ToLower());
            }
        }

        private void LoadEngines() {
            JArray servers = Config.GetValue("Engines", StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
            if(servers == null) return;
            foreach(JObject server in servers) {
                string EngineName = server.GetValue("Engine", StringComparison.CurrentCultureIgnoreCase)?.Value<string>();
                JObject EngineConfig = server.GetValue("Config", StringComparison.CurrentCultureIgnoreCase)?.Value<JObject>();
                if(EngineName == null) continue;
                if(AvailableEngines.ContainsKey(EngineName.ToLower())) {
                    Type type = AvailableEngines[EngineName.ToLower()];
                    _Logger.Info($"Creating Engine: '{type.Name}'");
                    Engine instance = (Engine)Activator.CreateInstance(type);
                    instance.SetConfig(EngineConfig);
                    Engines.Add(instance);
                } else {
                    _Logger.Info($"Unable to create, Engine '{EngineName}' does not exist.");
                }
            }
        }
    }
}
