using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace HtcPlugin.LuaProcessor.Lua {
    [MoonSharpUserData]
    public class LuaLowLevelAccess {
        private readonly Dictionary<string, LuaLowLevelClass> _lowLevelClasses;
        private readonly Dictionary<string, Dictionary<string, object>> _lowLevelScriptsMemory;
        private readonly List<string> _lowLevelScriptSessions;

        public LuaLowLevelAccess() {
            _lowLevelClasses = new Dictionary<string, LuaLowLevelClass>(0);
            _lowLevelScriptsMemory = new Dictionary<string, Dictionary<string, object>>(0);
            _lowLevelScriptSessions = new List<string>(0);
        }

        [MoonSharpVisible(true)]
        public object GetLowLevelClasses(string key) => _lowLevelClasses[key];

        [MoonSharpVisible(false)]
        public void RegisterLowLevelClass(string key, LuaLowLevelClass value) {
            Console.WriteLine($"Registering class: {key} -> {value.GetType().FullName}");
            _lowLevelClasses.Add(key, value);
        }

        [MoonSharpVisible(false)]
        public void RemoveLowLevelClass(string key) => _lowLevelClasses.Remove(key);

        [MoonSharpVisible(false)]
        public void ClearLowLevelClasses() => _lowLevelClasses.Clear();

        [MoonSharpVisible(false)]
        public Dictionary<string, object> GetLowScriptMemory(string key) => _lowLevelScriptsMemory[key];

        [MoonSharpVisible(false)]
        public string StartSession() {
            var guid = Guid.NewGuid().ToString();
            _lowLevelScriptSessions.Add(guid);
            _lowLevelScriptsMemory.Add(guid, new Dictionary<string, object>(0));
            return guid;
        }

        [MoonSharpVisible(false)]
        public void CloseSession(string key) {
            _lowLevelScriptSessions.Remove(key);
            _lowLevelScriptsMemory[key].Clear();
            _lowLevelScriptsMemory.Remove(key);
        }
    }
}
