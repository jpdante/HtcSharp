using System;

namespace HtcSharp.Core.Logging.Abstractions {
    public class NullLogger : ILogger {
        public void Log(LogLevel logLevel, object obj, Exception ex) {
            
        }

        public bool IsEnabled(LogLevel logLevel) {
            return false;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}