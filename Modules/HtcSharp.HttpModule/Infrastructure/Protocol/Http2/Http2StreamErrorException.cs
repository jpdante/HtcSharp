using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    internal sealed class Http2StreamErrorException : Exception {
        public Http2StreamErrorException(int streamId, string message, Http2ErrorCode errorCode)
            : base($"HTTP/2 stream ID {streamId} error ({errorCode}): {message}") {
            StreamId = streamId;
            ErrorCode = errorCode;
        }

        public int StreamId { get; }

        public Http2ErrorCode ErrorCode { get; }
    }
}
