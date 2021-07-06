using System;

namespace HtcSharp.Logging.Internal {
    [Flags]
    public enum LogLevel {
        Debug = 1,
        Info = 2,
        Warn = 4,
        Error = 8,
        Fatal = 16,
        All = 31,
    }
}