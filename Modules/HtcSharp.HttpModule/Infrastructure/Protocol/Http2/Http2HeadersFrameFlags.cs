using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    [Flags]
    internal enum Http2HeadersFrameFlags : byte {
        NONE = 0x0,
        END_STREAM = 0x1,
        END_HEADERS = 0x4,
        PADDED = 0x8,
        PRIORITY = 0x20
    }
}
