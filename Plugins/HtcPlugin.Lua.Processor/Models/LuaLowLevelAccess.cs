using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace HtcPlugin.Lua.Processor.Models {
    [MoonSharpUserData]
    public class LuaLowLevelAccess {
        public readonly Dictionary<string, LuaLowLevelClass> _lowLevelClasses;
        private readonly List<string> _lowLevelScriptSessions;

        public LuaLowLevelAccess() {
            _lowLevelClasses = new Dictionary<string, LuaLowLevelClass>(0);
            _lowLevelScriptSessions = new List<string>(0);
        }

        [MoonSharpVisible(true)]
        public object GetLowLevelClass(string key) => _lowLevelClasses[key];

        [MoonSharpVisible(false)]
        public void RegisterLowLevelClass(string key, LuaLowLevelClass value) => _lowLevelClasses.Add(key, value);

        [MoonSharpVisible(false)]
        public void RemoveLowLevelClass(string key) => _lowLevelClasses.Remove(key);

        [MoonSharpVisible(false)]
        public void ClearLowLevelClasses() => _lowLevelClasses.Clear();
    }
}
