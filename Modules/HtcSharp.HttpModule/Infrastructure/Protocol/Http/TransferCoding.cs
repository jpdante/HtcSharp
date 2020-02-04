using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    [Flags]
    internal enum TransferCoding {
        None,
        Chunked,
        Other
    }
}
