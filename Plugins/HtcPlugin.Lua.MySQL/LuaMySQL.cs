using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin.Abstractions;

namespace HtcPlugin.Lua.MySql {
    public class LuaMySql : IPlugin {

        public string Name => "HtcLuaMySql";
        public string Version => "0.1.2";

        internal MySqlRegister MySqlRegister;
        internal ILogger Logger;

        public Task Load(ILogger logger) {
            Logger = logger;
            return Task.CompletedTask;
        }

        public Task Enable() {
            MySqlRegister = new MySqlRegister(this);
            return Task.CompletedTask;
        }

        public Task Disable() {
            MySqlRegister.Uninitialized();
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }
    }
}
