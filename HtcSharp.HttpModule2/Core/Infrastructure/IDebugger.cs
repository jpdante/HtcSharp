using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    internal interface IDebugger {
        bool IsAttached { get; }
    }
}