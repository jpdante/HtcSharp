using System;

namespace HtcSharp.HttpModule2.Connection.Exceptions {
    internal sealed class HPackDecodingException : Exception {
        public HPackDecodingException(string message) : base(message) {
        }
        public HPackDecodingException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
