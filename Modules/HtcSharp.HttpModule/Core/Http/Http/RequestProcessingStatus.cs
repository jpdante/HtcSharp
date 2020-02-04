namespace HtcSharp.HttpModule.Core.Http.Http {
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
