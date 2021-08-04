using System;

namespace HtcSharp.Logging {
    [Flags]
    public enum LogLevel {
        None = 0,
        Debug = 1,
        Info = 2,
        Warn = 4,
        Error = 8,
        Fatal = 16,

        All = 31,
        DebugUp = 31,
        InfoUp = 30,
        WarnUp = 28,
        ErrorUp = 24,
        FatalUp = 16
    }
}