namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    internal enum RequestProcessingStatus {
        RequestPending,
        ParsingRequestLine,
        ParsingHeaders,
        AppStarted,
        HeadersCommitted,
        HeadersFlushed,
        ResponseCompleted
    }
}
