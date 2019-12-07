using System;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    internal interface ISystemClock {
        DateTimeOffset UtcNow { get; }

        long UtcNowTicks { get; }

        DateTimeOffset UtcNowUnsynchronized { get; }
    }
}