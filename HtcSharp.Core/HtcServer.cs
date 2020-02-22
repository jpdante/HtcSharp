using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Core.Engine;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Module;
using HtcSharp.Core.Plugin;

namespace HtcSharp.Core {
    public class HtcServer {

        private ILogger _logger;
        private EngineManager _engineManager;
        private ModuleManager _moduleManager;
        private PluginManager _pluginManager;

        public HtcServer() {
            _logger = new MultiLogger();
            _engineManager = new EngineManager(_logger);
            _moduleManager = new ModuleManager(this, _logger);
            _pluginManager = new PluginManager(_logger);
        }

        public async Task Start() {

        }

        public async Task Stop() {

        }

        /*private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, Type> _availableEngines;
        private List<Engine> _engines;
        private JObject _config;
        private readonly string _configPath;
        private string _errorPagesPath;

        public static bool IsDebug { get; private set; }
        public static HtcServer Context { get; private set; }
        public PluginManager PluginsManager { get; private set; }
        public string PluginsPath { get; private set; }

        public T GetConfig<T>(string key) { 
            return _config.GetValue(key, StringComparison.CurrentCultureIgnoreCase).Value<T>();
        }

        public T GetConfig<T>(string fileName, string key) { 
            return IoUtils.GetJsonFile(fileName).GetValue(key, StringComparison.CurrentCultureIgnoreCase).Value<T>();
        }

        public string AspNetConfigPath { get; private set; }
        public bool IsStopped { get; private set; }
        public ReadOnlyCollection<Engine> GetEngines() { return _engines.AsReadOnly(); }

        public HtcServer(string configPath) {
            Context = this;
            _configPath = configPath;
            IsStopped = false;
            IsDebug = false;
            _engines = new List<Engine>();
        }

        public void Start() {
            _availableEngines = new Dictionary<string, Type>();
            UrlMapper.RegisterIndexFile("index.html");
            UrlMapper.RegisterIndexFile("index.htm");
            RegisterEngine("http", typeof(HttpEngine));
            _engines = new List<Engine>();
            Logger.Info("Starting HtcSharp...");
            Logger.Info("Loading Configuration...");
            AspNetConfigPath = Path.Combine(Path.GetDirectoryName(_configPath), "aspnet-appsettings.json");
            try {
                if (!File.Exists(_configPath)) IoUtils.CreateHtcConfig(_configPath);
                _config = IoUtils.GetJsonFile(_configPath);
                if(!File.Exists(AspNetConfigPath)) IoUtils.CreateAspConfig(AspNetConfigPath);
                PluginsPath = _config.GetValue("PluginsPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>() ?? Path.Combine(Directory.GetCurrentDirectory(), @"plugins/");
                PluginsPath = IoUtils.ReplacePathTags(PluginsPath);
                _errorPagesPath = _config.GetValue("ErrorPagesPath", StringComparison.CurrentCultureIgnoreCase)?.Value<string>() ?? Path.Combine(Directory.GetCurrentDirectory(), @"error-pages/");
                _errorPagesPath = IoUtils.ReplacePathTags(_errorPagesPath);
                IsDebug = _config.GetValue("Debug", StringComparison.CurrentCultureIgnoreCase)?.Value<bool>() == true;
                if (!Directory.Exists(PluginsPath)) Directory.CreateDirectory(PluginsPath);
                if (!Directory.Exists(_errorPagesPath)) Directory.CreateDirectory(_errorPagesPath);
            } catch (Exception ex) {
                Logger.Error("Failed to load configuration!", ex);
                return;
            }
            RegisterErrorPages();
            PluginsManager = new PluginManager();
            PluginsManager.LoadPlugins(PluginsPath);
            PluginsManager.Call_OnLoad();
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
            PluginsManager.Call_OnEnable();
            Logger.Info("HTCServer is now running!");
        }

        public void Stop() {
            Logger.Info("Shutting down HtcSharp...");
            PluginsManager.Call_OnDisable();
            IsStopped = false;
            foreach (var engine in _engines) {
                try {
                    Logger.Info($"Stopping Engine: '{engine.GetType().Name}'");
                    engine.Stop();
                } catch (Exception ex) {
                    Logger.Error($"Failed to stop Engine: '{engine.GetType().Name}'", ex);
                }
            }
            _engines.Clear();
            _availableEngines.Clear();
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

        private void RegisterErrorPages() {
            var path400 = Path.Combine(_errorPagesPath, "400.html");
            var path401 = Path.Combine(_errorPagesPath, "401.html");
            var path403 = Path.Combine(_errorPagesPath, "403.html");
            var path404 = Path.Combine(_errorPagesPath, "404.html");
            var path500 = Path.Combine(_errorPagesPath, "500.html");
            var path501 = Path.Combine(_errorPagesPath, "501.html");
            var path502 = Path.Combine(_errorPagesPath, "502.html");
            var path503 = Path.Combine(_errorPagesPath, "503.html");
            if(!File.Exists(path400)) File.WriteAllText(path400, IoUtils.Default400);
            if(!File.Exists(path401)) File.WriteAllText(path401, IoUtils.Default401);
            if(!File.Exists(path403)) File.WriteAllText(path403, IoUtils.Default403);
            if(!File.Exists(path404)) File.WriteAllText(path404, IoUtils.Default404);
            if(!File.Exists(path500)) File.WriteAllText(path500, IoUtils.Default500);
            if(!File.Exists(path501)) File.WriteAllText(path501, IoUtils.Default501);
            if(!File.Exists(path502)) File.WriteAllText(path502, IoUtils.Default502);
            if(!File.Exists(path503)) File.WriteAllText(path503, IoUtils.Default503);
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path400, 400));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path401, 401));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path403, 403));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path404, 404));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path500, 500));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path501, 501));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path502, 502));
            ErrorMessagesManager.RegisterDefaultPage(new FilePageMessage(path503, 503));
        }

        public void RegisterEngine(string engineName, Type type) {
            if (!_availableEngines.ContainsKey(engineName.ToLower())) _availableEngines.Add(engineName, type);
            else {
                throw new KeyAlreadyExistException(engineName.ToLower());
            }
        }

        public void UnRegisterEngine(string engineName) {
            if(_availableEngines.ContainsKey(engineName.ToLower())) _availableEngines.Remove(engineName);
            else {
                throw new KeyNotFoundException(engineName.ToLower());
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
        }*/
    }
}
