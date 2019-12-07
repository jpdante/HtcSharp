using System;

namespace HtcSharp.HttpModule2.Core.Http2.Flags {
    [Flags]
    internal enum Http2DataFrameFlags : byte {
        NONE = 0x0,
        END_STREAM = 0x1,
        PADDED = 0x8
    }
}
