namespace HtcSharp.HttpModule.Core.Infrastructure {
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
