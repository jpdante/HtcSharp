using System;

namespace HtcSharp.HttpModule2.Core.Http {
    [Flags]
    internal enum TransferCoding
    {
        None,
        Chunked,
        Other
    }
}