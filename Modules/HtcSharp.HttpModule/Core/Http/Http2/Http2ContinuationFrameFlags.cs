using System;

namespace HtcSharp.HttpModule.Core.Http.Http2 {
    [Flags]
    internal enum Http2ContinuationFrameFlags : byte {
        NONE = 0x0,
        END_HEADERS = 0x4,
    }
}
