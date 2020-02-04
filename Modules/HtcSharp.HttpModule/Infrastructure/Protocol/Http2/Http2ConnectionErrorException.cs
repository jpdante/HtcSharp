using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    internal sealed class Http2ConnectionErrorException : Exception {
        public Http2ConnectionErrorException(string message, Http2ErrorCode errorCode)
            : base($"HTTP/2 connection error ({errorCode}): {message}") {
            ErrorCode = errorCode;
        }

        public Http2ErrorCode ErrorCode { get; }
    }
}
