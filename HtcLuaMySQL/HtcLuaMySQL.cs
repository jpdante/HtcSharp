using System;
using System.Reflection;
using HtcPlugin.LuaProcessor;
using HtcSharp.Core;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using MoonSharp.Interpreter;

namespace HtcPlugin.LuaMySql {
    public class HtcLuaMySql : IHtcPlugin {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        public string PluginName => "HtcLuaMySql";
        public string PluginVersion => "1.0";

        public void OnLoad() {
            UserData.RegisterType<MySqlClass>();
        }

        public void OnEnable() {
            Logger.Info("Starting HtcPlugin.LuaMySql");
            Logger.Info($"Injecting => {HtcLuaProcessor.Context}");
        }

        public void OnDisable() {
            
        }
    }
}
