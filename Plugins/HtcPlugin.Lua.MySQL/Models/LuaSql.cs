using System;
using System.Collections.Generic;
using System.Text;
using HtcPlugin.Lua.Processor.Models;

namespace HtcPlugin.Lua.MySql.Models {
    public class LuaSql : LuaLowLevelClass {
        public LuaSqlConnection NewConnection() {
            return new LuaSqlConnection();
        }

        public LuaSqlConnection NewConnection(string connectionString) {
            return new LuaSqlConnection(connectionString);
        }

        public LuaSqlCommand NewCommand() {
            return new LuaSqlCommand();
        }

        public LuaSqlCommand NewCommand(string commandText) {
            return new LuaSqlCommand(commandText);
        }

        public LuaSqlCommand NewCommand(string commandText, LuaSqlConnection connection) {
            return new LuaSqlCommand(commandText, connection);
        }

        public LuaSqlCommand NewCommand(string commandText, LuaSqlConnection connection, LuaSqlTransaction transaction) {
            return new LuaSqlCommand(commandText, connection, transaction);
        }
    }
}
