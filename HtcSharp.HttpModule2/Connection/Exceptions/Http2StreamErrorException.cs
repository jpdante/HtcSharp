using System;
using HtcSharp.HttpModule2.Core.Http2;

namespace HtcSharp.HttpModule2.Connection.Exceptions {
    internal sealed class Http2StreamErrorException : Exception {
        public Http2StreamErrorException(int streamId, string message, Http2ErrorCode errorCode) : base($"HTTP/2 stream ID {streamId} error ({errorCode}): {message}") {
            StreamId = streamId;
            ErrorCode = errorCode;
        }

        public int StreamId { get; }

        public Http2ErrorCode ErrorCode { get; }
    }
}
