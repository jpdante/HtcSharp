using System;

namespace HtcSharp.HttpModule.Core.Http.Http {
    [Flags]
    internal enum TransferCoding {
        None,
        Chunked,
        Other
    }
}
