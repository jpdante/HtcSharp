using System;

namespace HtcSharp.HttpModule2.Core.Http2.Flags {
    [Flags]
    internal enum Http2PingFrameFlags : byte {
        NONE = 0x0,
        ACK = 0x1
    }
}
