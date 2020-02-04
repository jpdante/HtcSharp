using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2.HPack {
    internal sealed class HPackEncodingException : Exception {
        public HPackEncodingException(string message)
            : base(message) {
        }
        public HPackEncodingException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
