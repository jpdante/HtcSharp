using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace HtcPlugin.Lua.MySql.Models {
    public class LuaSqlDataReader {
        private readonly MySqlDataReader _dataReader;

        public LuaSqlDataReader(MySqlDataReader dataReader) {
            _dataReader = dataReader;
        }

        public int Depth => _dataReader.Depth;
        public int FieldCount => _dataReader.FieldCount;
        public bool HasRows => _dataReader.HasRows;
        public bool IsClosed => _dataReader.IsClosed;
        public int RecordsAffected => _dataReader.RecordsAffected;
        public int VisibleFieldCount => _dataReader.VisibleFieldCount;

		public bool GetBoolean(int ordinal) => _dataReader.GetBoolean(ordinal);
		public byte GetByte(int ordinal) => _dataReader.GetByte(ordinal);
		public sbyte GetSByte(int ordinal) => _dataReader.GetSByte(ordinal);
		public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => _dataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
		public char GetChar(int ordinal) => _dataReader.GetChar(ordinal);
		public long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)=> _dataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
		public Guid GetGuid(int ordinal) => _dataReader.GetGuid(ordinal);
		public short GetInt16(int ordinal) => _dataReader.GetInt16(ordinal);
		public int GetInt32(int ordinal) => _dataReader.GetInt32(ordinal);
		public long GetInt64(int ordinal) => _dataReader.GetInt64(ordinal);
		public string GetDataTypeName(int ordinal) => _dataReader.GetDataTypeName(ordinal);
		public Type GetFieldType(int ordinal) => _dataReader.GetFieldType(ordinal);
		public object GetValue(int ordinal) => _dataReader.GetValue(ordinal);
		public IEnumerator GetEnumerator() => _dataReader.GetEnumerator();
		public  DateTime GetDateTime(int ordinal) => _dataReader.GetDateTime(ordinal);
		public DateTimeOffset GetDateTimeOffset(int ordinal) => _dataReader.GetDateTimeOffset(ordinal);
		public MySqlDateTime GetMySqlDateTime(int ordinal) => _dataReader.GetMySqlDateTime(ordinal);
		public TimeSpan GetTimeSpan(int ordinal) => (TimeSpan) GetValue(ordinal);
		public Stream GetStream(int ordinal) => _dataReader.GetStream(ordinal);
		public TextReader GetTextReader(int ordinal) => new StringReader(GetString(ordinal));
		public TextReader GetTextReader(string name) => new StringReader(GetString(name));
		public string GetString(int ordinal) => _dataReader.GetString(ordinal);
		public decimal GetDecimal(int ordinal) => _dataReader.GetDecimal(ordinal);
		public double GetDouble(int ordinal) => _dataReader.GetDouble(ordinal);
		public float GetFloat(int ordinal) => _dataReader.GetFloat(ordinal);
		public ushort GetUInt16(int ordinal) => _dataReader.GetUInt16(ordinal);
		public uint GetUInt32(int ordinal) => _dataReader.GetUInt32(ordinal);
		public ulong GetUInt64(int ordinal) => _dataReader.GetUInt64(ordinal);


        public int GetOrdinal(string name) => _dataReader.GetOrdinal(name);
        public bool GetBoolean(string name) => _dataReader.GetBoolean(name);
        public byte GetByte(string name) => _dataReader.GetByte(name);
        public sbyte GetSByte(string name) => _dataReader.GetSByte(name);
        public char GetChar(string name) => _dataReader.GetChar(name);
        public Guid GetGuid(string name) => _dataReader.GetGuid(name);
        public short GetInt16(string name) => _dataReader.GetInt16(name);
        public int GetInt32(string name) => _dataReader.GetInt32(name);
        public long GetInt64(string name) => _dataReader.GetInt64(name);
        public Type GetFieldType(string name) => _dataReader.GetFieldType(name);
        public DateTime GetDateTime(string name) => _dataReader.GetDateTime(name);
        public DateTimeOffset GetDateTimeOffset(string name) => _dataReader.GetDateTimeOffset(name);
        public MySqlDateTime GetMySqlDateTime(string name) => _dataReader.GetMySqlDateTime(name);
        public TimeSpan GetTimeSpan(string name) => _dataReader.GetTimeSpan(name);
        public Stream GetStream(string name) => _dataReader.GetStream(name);
        public string GetString(string name) => _dataReader.GetString(name);
        public decimal GetDecimal(string name) => _dataReader.GetDecimal(name);
        public double GetDouble(string name) => _dataReader.GetDouble(name);
        public float GetFloat(string name) => _dataReader.GetFloat(name);
        public ushort GetUInt16(string name) => _dataReader.GetUInt16(name);
        public uint GetUInt32(string name) => _dataReader.GetUInt32(name);
        public ulong GetUInt64(string name) => _dataReader.GetUInt64(name);

        public void Close() => _dataReader.Close();
        public bool IsDBNull(int ordinal) => _dataReader.IsDBNull(ordinal);
        public bool NextResult() => _dataReader.NextResult();
        public bool Read() => _dataReader.Read();
    }
}
