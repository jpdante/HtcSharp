using System;

namespace HtcSharp.HttpModule.Infrastructure {
    [Flags]
    public enum HttpProtocols {
        None = 0x0,
        Http1 = 0x1,
        Http2 = 0x2,
        Http1AndHttp2 = Http1 | Http2,
    }
}
