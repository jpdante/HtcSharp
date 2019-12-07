using HtcSharp.HttpModule2.Core.Http2.Flags;

namespace HtcSharp.HttpModule2.Core.Http2.Frame {
    internal partial class Http2Frame {
        public Http2ContinuationFrameFlags ContinuationFlags {
            get => (Http2ContinuationFrameFlags)Flags;
            set => Flags = (byte)value;
        }

        public bool ContinuationEndHeaders => (ContinuationFlags & Http2ContinuationFrameFlags.END_HEADERS) == Http2ContinuationFrameFlags.END_HEADERS;

        public void PrepareContinuation(Http2ContinuationFrameFlags flags, int streamId) {
            PayloadLength = 0;
            Type = Http2FrameType.CONTINUATION;
            ContinuationFlags = flags;
            StreamId = streamId;
        }
    }
}
