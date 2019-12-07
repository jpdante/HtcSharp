using HtcSharp.HttpModule2.Core.Http2.Flags;

namespace HtcSharp.HttpModule2.Core.Http2.Frame {
    internal partial class Http2Frame {
        public Http2HeadersFrameFlags HeadersFlags {
            get => (Http2HeadersFrameFlags)Flags;
            set => Flags = (byte)value;
        }

        public bool HeadersEndHeaders => (HeadersFlags & Http2HeadersFrameFlags.END_HEADERS) == Http2HeadersFrameFlags.END_HEADERS;

        public bool HeadersEndStream => (HeadersFlags & Http2HeadersFrameFlags.END_STREAM) == Http2HeadersFrameFlags.END_STREAM;

        public bool HeadersHasPadding => (HeadersFlags & Http2HeadersFrameFlags.PADDED) == Http2HeadersFrameFlags.PADDED;

        public bool HeadersHasPriority => (HeadersFlags & Http2HeadersFrameFlags.PRIORITY) == Http2HeadersFrameFlags.PRIORITY;

        public byte HeadersPadLength { get; set; }

        public int HeadersStreamDependency { get; set; }

        public byte HeadersPriorityWeight { get; set; }

        private int HeadersPayloadOffset => (HeadersHasPadding ? 1 : 0) + (HeadersHasPriority ? 5 : 0);

        public int HeadersPayloadLength => PayloadLength - HeadersPayloadOffset - HeadersPadLength;

        public void PrepareHeaders(Http2HeadersFrameFlags flags, int streamId) {
            PayloadLength = 0;
            Type = Http2FrameType.HEADERS;
            HeadersFlags = flags;
            StreamId = streamId;
        }
    }
}
