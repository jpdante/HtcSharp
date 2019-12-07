namespace HtcSharp.HttpModule2.Core.Http2.Frame {
    internal partial class Http2Frame {
        public int GoAwayLastStreamId { get; set; }

        public Http2ErrorCode GoAwayErrorCode { get; set; }

        public void PrepareGoAway(int lastStreamId, Http2ErrorCode errorCode) {
            PayloadLength = 8;
            Type = Http2FrameType.GOAWAY;
            Flags = 0;
            StreamId = 0;
            GoAwayLastStreamId = lastStreamId;
            GoAwayErrorCode = errorCode;
        }
    }
}