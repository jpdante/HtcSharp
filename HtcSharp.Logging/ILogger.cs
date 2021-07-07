using System;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging {
    public interface ILogger {

        public Type Type { get; }

        void Log(LogLevel logLevel, string msg, params object[] objs);
        void Log(LogLevel logLevel, string msg, Exception ex, params object[] objs);
        bool IsEnabled(LogLevel logLevel);

    }
}
