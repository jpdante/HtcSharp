using System;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class NullAppender : IAppender {

        public void Log(ILogger logger, LogLevel logLevel, object msg, params object[] objs) { }

        public void Log(ILogger logger, LogLevel logLevel, object msg, Exception ex, params object[] objs) { }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public void Dispose() {
        }
    }
}