using HTCSharp.Core.Engines;
using HTCSharp.Core.IO;
using HTCSharp.Core.Logging;
using HTCSharp.Core.Managers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private string ConfigPath;
        private string PluginsPath;

        public static bool IsDebug { get { return _Debug; } }

        public bool IsStopped { get { return Active; } }

        public HTCServer(string configPath) {
            ConfigPath = configPath;
            Active = false;
            _Debug = false;
        }

        public void Start() {
            AvailableEngines = new Dictionary<string, Type>();
            RegisterEngine("http", typeof(HttpEngine));
            Engines = new List<Engine>();
            _Logger.Info("Starting HTCSharp...");
            _Logger.Info("Loading Configuration...");
            try {
                Config = HTCFile.GetJsonFile(ConfigPath);
                PluginsPath = Config.GetValue("PluginsPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>() ?? HTCDirectory.CombinePath(HTCDirectory.GetWorkingDirectory(), @"plugins\");
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
                _Logger.Info($"Starting Engine: '{engine.GetType().Name}'");
                engine.Start();
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
                _Logger.Info($"Stopping Engine: '{engine.GetType().Name}'");
                engine.Stop();
            }
            this.Engines.Clear();
            this.AvailableEngines.Clear();
        }

        public void WaitStop() {
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
            JArray servers = Config.GetValue("Servers", StringComparison.CurrentCultureIgnoreCase)?.Value<JArray>();
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
                }
            }
        }
    }
}
