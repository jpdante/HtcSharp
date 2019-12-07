using System;

namespace HtcSharp.HttpModule2.Core.Http {
    [Flags]
    internal enum ConnectionOptions {
        NONE = 0,
        CLOSE = 1,
        KEEP_ALIVE = 2,
        UPGRADE = 4
    }
}
