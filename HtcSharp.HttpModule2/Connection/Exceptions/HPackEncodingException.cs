using System;

namespace HtcSharp.HttpModule2.Connection.Exceptions {
    internal sealed class HPackEncodingException : Exception {
        public HPackEncodingException(string message)
            : base(message) {
        }
        public HPackEncodingException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}