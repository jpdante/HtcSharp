namespace HtcSharp.HttpModule2.Core.Http2.Frame {
    internal partial class Http2Frame {
        public Http2ErrorCode RstStreamErrorCode { get; set; }

        public void PrepareRstStream(int streamId, Http2ErrorCode errorCode) {
            PayloadLength = 4;
            Type = Http2FrameType.RST_STREAM;
            Flags = 0;
            StreamId = streamId;
            RstStreamErrorCode = errorCode;
        }
    }
}