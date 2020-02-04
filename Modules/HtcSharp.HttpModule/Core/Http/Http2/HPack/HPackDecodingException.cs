using System;

namespace HtcSharp.HttpModule.Core.Http.Http2.HPack {
    internal sealed class HPackDecodingException : Exception {
        public HPackDecodingException(string message)
            : base(message) {
        }
        public HPackDecodingException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
