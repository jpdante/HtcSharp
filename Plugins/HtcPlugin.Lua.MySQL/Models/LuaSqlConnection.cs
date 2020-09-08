using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter.Interop;
using MySqlConnector;

namespace HtcPlugin.Lua.MySql.Models {
    public class LuaSqlConnection {
        [MoonSharpVisible(false)]
        public MySqlConnection SqlConnection;

        [MoonSharpVisible(false)]
        public LuaSqlConnection() {
            SqlConnection = new MySqlConnection();
        }

        [MoonSharpVisible(false)]
        public LuaSqlConnection(string connectionString) {
            SqlConnection = new MySqlConnection(connectionString);
        }

        public int ConnectionTimeout => SqlConnection.ConnectionTimeout;
        public int ServerThread => SqlConnection.ServerThread;
        public string ConnectionString => SqlConnection.ConnectionString;
        public string Database => SqlConnection.Database;
        public string State => SqlConnection.State.ToString();
        public string DataSource => SqlConnection.DataSource;
        public string ServerVersion => SqlConnection.ServerVersion;

        public void Open() => SqlConnection.Open();
        public void Close() => SqlConnection.Close();
        public void ChangeDatabase(string databaseName) => SqlConnection.ChangeDatabase(databaseName);
        public bool Ping() => SqlConnection.Ping();
        public LuaSqlTransaction BeginTransaction() => new LuaSqlTransaction(SqlConnection.BeginTransaction(), this);
    }
}
