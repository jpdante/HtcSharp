using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.HttpModule2.Core.Http {

    public enum HttpMethod : byte {
        Get,
        Put,
        Delete,
        Post,
        Head,
        Trace,
        Patch,
        Connect,
        Options,

        Custom,

        None = byte.MaxValue,
    }
}
