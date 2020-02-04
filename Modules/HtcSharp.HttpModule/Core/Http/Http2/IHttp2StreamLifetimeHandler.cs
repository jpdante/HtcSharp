namespace HtcSharp.HttpModule.Core.Http.Http2 {
    internal interface IHttp2StreamLifetimeHandler {
        void OnStreamCompleted(Http2Stream stream);
        void DecrementActiveClientStreamCount();
    }
}
