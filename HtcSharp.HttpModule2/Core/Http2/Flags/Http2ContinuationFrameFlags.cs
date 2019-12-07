using System;

namespace HtcSharp.HttpModule2.Core.Http2.Flags {
    [Flags]
    internal enum Http2ContinuationFrameFlags : byte {
        NONE = 0x0,
        END_HEADERS = 0x4,
    }
}
