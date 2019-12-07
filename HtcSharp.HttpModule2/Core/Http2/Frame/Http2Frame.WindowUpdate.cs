namespace HtcSharp.HttpModule2.Core.Http2.Frame {
    internal partial class Http2Frame {
        public int WindowUpdateSizeIncrement { get; set; }

        public void PrepareWindowUpdate(int streamId, int sizeIncrement) {
            PayloadLength = 4;
            Type = Http2FrameType.WINDOW_UPDATE;
            Flags = 0;
            StreamId = streamId;
            WindowUpdateSizeIncrement = sizeIncrement;
        }
    }
}