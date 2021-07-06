using System;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Loggers {
    public class ConsoleLogger : ILogger {

        private LogLevel _logLevels;
        private readonly LogFormatter _logFormatter;

        public ConsoleLogger(LogLevel logLevels, LogFormatter logFormatter = null) {
            _logLevels = logLevels;
            _logFormatter = logFormatter;
            _logFormatter ??= new LogFormatter();
        }

        public void Log(LogLevel logLevel, string msg, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            Console.Write(_logFormatter.FormatLog(logLevel, msg, objs));
        }

        public void Log(LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            Console.Write(_logFormatter.FormatLog(logLevel, msg, ex, objs));
        }

        public bool IsEnabled(LogLevel logLevel) => _logLevels.HasFlag(logLevel);

        public void SetLogLevel(LogLevel logLevels) => _logLevels = logLevels;
    }
}