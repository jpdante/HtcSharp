namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    /* https://tools.ietf.org/html/rfc7540#section-6.7
        +---------------------------------------------------------------+
        |                                                               |
        |                      Opaque Data (64)                         |
        |                                                               |
        +---------------------------------------------------------------+
    */
    internal partial class Http2Frame {
        public Http2PingFrameFlags PingFlags {
            get => (Http2PingFrameFlags)Flags;
            set => Flags = (byte)value;
        }

        public bool PingAck => (PingFlags & Http2PingFrameFlags.ACK) == Http2PingFrameFlags.ACK;

        public void PreparePing(Http2PingFrameFlags flags) {
            PayloadLength = 8;
            Type = Http2FrameType.PING;
            PingFlags = flags;
            StreamId = 0;
        }
    }
}
