using System;

namespace HtcSharp.Core.Logging.Abstractions {
    public abstract class FormattedLogger : ILogger {

        public string FormatLog(LogLevel logLevel, object obj, Exception ex) {
            if (IsEnabled(LogLevel.Trace)) {
                return $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss,fff}] [{logLevel.ToString()}] {obj} {ex?.Message} {Environment.NewLine} {ex?.StackTrace}";
            } else {
                return $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss,fff}] [{logLevel.ToString()}] {obj} {ex?.Message}";
            }
        }

        public abstract void Dispose();

        public abstract void Log(LogLevel logLevel, object obj, Exception ex);

        public abstract bool IsEnabled(LogLevel logLevel);
    }
}
