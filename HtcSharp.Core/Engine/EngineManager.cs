using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Engine.Abstractions;
using HtcSharp.Core.Logging.Abstractions;
using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Engine {
    public class EngineManager {

        private readonly ILogger _logger;
        private readonly List<IEngine> _engines;
        private readonly Dictionary<string, Type> _availableEngines;
        private readonly List<string> _engineNames;

        public EngineManager(ILogger logger) {
            _logger = logger;
            _engines = new List<IEngine>();
            _engineNames = new List<string>();
            _availableEngines = new Dictionary<string, Type>();
        }

        public void RegisterEngine(string name, Type type) {
            if(type == null) return;
            if (!type.IsAssignableFrom(typeof(IEngine))) return;
            _availableEngines.Add(name.ToLower(), type);
            _engineNames.Add(name.ToLower());
        }

        public void UnRegisterEngine(string name) {
            _availableEngines.Remove(name.ToLower());
            _engineNames.Remove(name.ToLower());
        }

        internal IEngine InstantiateEngine(string name) {
            if (!_availableEngines.TryGetValue(name, out var type)) return null;
            var engine = Activator.CreateInstance(type) as IEngine;
            return engine;
        }

        internal string[] GetEnginesNames() {
            return _engineNames.ToArray();
        }

        internal void AddEngine(IEngine engine) {
            _engines.Add(engine);
        }

        internal void RemoveEngine(IEngine engine) {
            _engines.Remove(engine);
        }

        internal async Task Load(IEngine engine, JObject config) {
            await engine.Load(config, _logger);
        }

        internal async Task Start() {
            foreach (var engine in _engines) {
                await engine.Start();
            }
        }

        internal async Task Stop() {
            foreach (var engine in _engines) {
                await engine.Stop();
            }
        }

    }
}
