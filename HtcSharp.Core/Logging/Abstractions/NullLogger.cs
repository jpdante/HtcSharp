using System;

namespace HtcSharp.Core.Logging.Abstractions {
    public class NullLogger : ILogger {
        public void Log(LogLevel logLevel, Type type, object obj, Exception ex) {
            
        }

        public bool IsEnabled(LogLevel logLevel) {
            return false;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}