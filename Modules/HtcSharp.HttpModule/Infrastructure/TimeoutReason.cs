namespace HtcSharp.HttpModule.Infrastructure {
    internal enum TimeoutReason {
        None,
        KeepAlive,
        RequestHeaders,
        ReadDataRate,
        WriteDataRate,
        RequestBodyDrain,
        TimeoutFeature,
    }
}
