using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter.Interop;
using MySqlConnector;

namespace HtcPlugin.Lua.MySql.Models {
    public class LuaSqlParameters {
        private readonly MySqlCommand _command;

        [MoonSharpVisible(false)]
        public LuaSqlParameters(MySqlCommand command) {
            _command = command;
        }

        public int Count => _command.Parameters.Count;

        public void AddWithValue(string parameterName, object value) => _command.Parameters.AddWithValue(parameterName, value);
        public void Clear() => _command.Parameters.Clear();
        public bool Contains(object value) => _command.Parameters.Contains(value);
        public int IndexOf(object value) => _command.Parameters.IndexOf(value);
        public void Remove(object value) => _command.Parameters.Remove(value);
        public void RemoveAt(int index) => _command.Parameters.RemoveAt(index);

    }
}
