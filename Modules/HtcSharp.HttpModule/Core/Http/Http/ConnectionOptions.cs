using System;

namespace HtcSharp.HttpModule.Core.Http.Http {
    [Flags]
    internal enum ConnectionOptions {
        None = 0,
        Close = 1,
        KeepAlive = 2,
        Upgrade = 4
    }
}
