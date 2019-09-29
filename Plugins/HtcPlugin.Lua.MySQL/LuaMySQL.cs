using System;
using System.Reflection;
using HtcPlugin.Lua.MySql.Models;
using HtcSharp.Core;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using MoonSharp.Interpreter;

namespace HtcPlugin.Lua.MySql {
    public class LuaMySQL : IHtcPlugin {
        public string PluginName => "HtcLuaMySql";
        public string PluginVersion => "0.1.2";
        private MySQLRegister _mySqlRegister;

        public void OnLoad() {

        }

        public void OnEnable() {
            _mySqlRegister = new MySQLRegister(this);
        }

        public void OnDisable() {
            _mySqlRegister.Uninitialized();
        }
    }
}
