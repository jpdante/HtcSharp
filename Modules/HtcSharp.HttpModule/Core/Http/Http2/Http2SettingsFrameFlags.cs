using System;

namespace HtcSharp.HttpModule.Core.Http.Http2 {
    [Flags]
    internal enum Http2SettingsFrameFlags : byte {
        NONE = 0x0,
        ACK = 0x1,
    }
}
