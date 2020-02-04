using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    [Flags]
    internal enum ConnectionOptions {
        None = 0,
        Close = 1,
        KeepAlive = 2,
        Upgrade = 4
    }
}
