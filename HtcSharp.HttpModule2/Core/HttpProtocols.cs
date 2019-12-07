using System;

namespace HtcSharp.HttpModule2.Core {
    [Flags]
    public enum HttpProtocols {
        NONE = 0x0,
        HTTP1 = 0x1,
        HTTP2 = 0x2,
        HTTP1_AND_HTTP2 = HTTP1 | HTTP2,
    }
}