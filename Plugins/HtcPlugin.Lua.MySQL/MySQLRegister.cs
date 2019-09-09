using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HtcPlugin.Lua.MySql.Models;
using HtcSharp.Core;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using MoonSharp.Interpreter;

namespace HtcPlugin.Lua.MySql {
    public class MySQLRegister {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Processor.LuaProcessor _luaProcessor;

        public MySQLRegister(IHtcPlugin pluginContext) {
            foreach (var plugin in HtcServer.Context.PluginsManager.GetPlugins) 
                if (plugin is Processor.LuaProcessor processor) _luaProcessor = processor;
            if (_luaProcessor == null) {
                Logger.Error("Failed to initialize HtcPlugin.Lua.MySql, dependency context not found.");
                HtcServer.Context.PluginsManager.UnloadPlugin(pluginContext);
                return;
            }
            UserData.RegisterType<LuaSql>();
            UserData.RegisterType<LuaSqlConnection>();
            UserData.RegisterType<LuaSqlCommand>();
            UserData.RegisterType<LuaSqlTransaction>();
            UserData.RegisterType<LuaSqlDataReader>();
            UserData.RegisterType<LuaSqlParameters>();
            _luaProcessor.LuaLowLevelAccess.RegisterLowLevelClass("mysql", new LuaSql());
        }

        public void Deinitialize() {
            _luaProcessor.LuaLowLevelAccess.RemoveLowLevelClass("mysql");
        }
    }
}
