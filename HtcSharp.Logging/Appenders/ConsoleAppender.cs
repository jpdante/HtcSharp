using System;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class ConsoleAppender : IAppender {

        private LogLevel _logLevels;
        private readonly IFormatter _logFormatter;

        public ConsoleAppender(LogLevel logLevels, IFormatter logFormatter = null) {
            _logLevels = logLevels;
            _logFormatter = logFormatter;
            _logFormatter ??= new Formatter();
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            Console.Write(_logFormatter.FormatLog(logger, logLevel, msg, null, objs));
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            Console.Write(_logFormatter.FormatLog(logger, logLevel, msg, ex, objs));
        }

        public bool IsEnabled(LogLevel logLevel) => _logLevels.HasFlag(logLevel);

        public void SetLogLevel(LogLevel logLevels) => _logLevels = logLevels;

        public void Dispose() {
        }
    }
}