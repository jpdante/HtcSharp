using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter.Interop;
using MySqlConnector;

namespace HtcPlugin.Lua.MySql.Models {
    public class LuaSqlTransaction {
        [MoonSharpVisible(false)]
        public MySqlTransaction Transaction;

        public LuaSqlConnection Connection { get; }

        [MoonSharpVisible(false)]
        public LuaSqlTransaction(MySqlTransaction transaction, LuaSqlConnection sqlConnection) {
            Transaction = transaction;
            Connection = sqlConnection;
        }

        public void Commit() {
            Transaction.Commit();
        }

        public void Rollback() {
            Transaction.Rollback();
        }
    }
}
