using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter.Interop;
using MySql.Data.MySqlClient;

namespace HtcPlugin.Lua.MySql.Models {
    public class LuaSqlCommand {
        private readonly MySqlCommand _command;
        private LuaSqlConnection _luaSqlConnection;
        private LuaSqlTransaction _luaSqlTransaction;

        public LuaSqlParameters Parameters { get; }

        [MoonSharpVisible(false)]
        public LuaSqlCommand() {
            _command = new MySqlCommand();
            Parameters = new LuaSqlParameters(_command);
        }

        [MoonSharpVisible(false)]
        public LuaSqlCommand(string commandText) {
            _command = new MySqlCommand(commandText);
            Parameters = new LuaSqlParameters(_command);
        }

        [MoonSharpVisible(false)]
        public LuaSqlCommand(string commandText, LuaSqlConnection connection) {
            _luaSqlConnection = connection;
            _command = new MySqlCommand(commandText, _luaSqlConnection.SqlConnection);
            Parameters = new LuaSqlParameters(_command);
        }

        [MoonSharpVisible(false)]
        public LuaSqlCommand(string commandText, LuaSqlConnection connection, LuaSqlTransaction transaction) {
            _luaSqlConnection = connection;
            _luaSqlTransaction = transaction;
            _command = new MySqlCommand(commandText, _luaSqlConnection.SqlConnection, _luaSqlTransaction.Transaction);
            Parameters = new LuaSqlParameters(_command);
        }

        public LuaSqlConnection Connection {
            get => _luaSqlConnection;
            set {
                _luaSqlConnection = value;
                _command.Connection = _luaSqlConnection.SqlConnection;
            }
        }

        public LuaSqlTransaction Transaction {
            get => _luaSqlTransaction;
            set {
                _luaSqlTransaction = value;
                _command.Transaction = _luaSqlTransaction.Transaction;
            }
        }

        public string CommandText => _command.CommandText;
        public int CommandTimeout => _command.CommandTimeout;
        public bool DesignTimeVisible => _command.DesignTimeVisible;
        public bool IsPrepared => _command.IsPrepared;
        public long LastInsertedId => _command.LastInsertedId;

        public int ExecuteNonQuery() => _command.ExecuteNonQuery();
        public void Prepare() => _command.Prepare();
        public object ExecuteScalar() => _command.ExecuteScalar();
        public LuaSqlDataReader ExecuteReader() => new LuaSqlDataReader(_command.ExecuteReader());
        public void Cancel() => _command.Cancel();
    }
}
