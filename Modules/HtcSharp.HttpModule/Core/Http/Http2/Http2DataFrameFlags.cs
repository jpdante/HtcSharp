using System;

namespace HtcSharp.HttpModule.Core.Http.Http2 {
    [Flags]
    internal enum Http2DataFrameFlags : byte {
        NONE = 0x0,
        END_STREAM = 0x1,
        PADDED = 0x8
    }
}
