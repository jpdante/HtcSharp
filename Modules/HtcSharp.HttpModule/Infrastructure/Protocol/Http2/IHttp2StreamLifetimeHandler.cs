namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    internal interface IHttp2StreamLifetimeHandler {
        void OnStreamCompleted(Http2Stream stream);
        void DecrementActiveClientStreamCount();
    }
}
