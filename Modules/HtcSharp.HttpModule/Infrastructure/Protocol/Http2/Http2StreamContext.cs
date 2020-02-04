using HtcSharp.HttpModule.Infrastructure.Protocol.Http2.FlowControl;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    internal sealed class Http2StreamContext : HttpConnectionContext {
        public int StreamId { get; set; }
        public IHttp2StreamLifetimeHandler StreamLifetimeHandler { get; set; }
        public Http2PeerSettings ClientPeerSettings { get; set; }
        public Http2PeerSettings ServerPeerSettings { get; set; }
        public Http2FrameWriter FrameWriter { get; set; }
        public InputFlowControl ConnectionInputFlowControl { get; set; }
        public OutputFlowControl ConnectionOutputFlowControl { get; set; }
    }
}
