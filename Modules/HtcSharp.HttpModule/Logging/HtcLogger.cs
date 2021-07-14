using System;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Logging {
    public class HtcLogger : ILogger {

        private readonly HtcSharp.Logging.ILogger _logger;

        public HtcLogger(HtcSharp.Logging.ILogger logger) {
            _logger = logger;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            _logger.Log(ConvertLogLevel(logLevel), $"[{eventId.Id}] [{eventId.Name}] {state}", exception);
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        private static HtcSharp.Logging.Internal.LogLevel ConvertLogLevel(LogLevel logLevel) {
            return logLevel switch {
                LogLevel.Trace => HtcSharp.Logging.Internal.LogLevel.Debug,
                LogLevel.Debug => HtcSharp.Logging.Internal.LogLevel.Debug,
                LogLevel.None => HtcSharp.Logging.Internal.LogLevel.Debug,
                LogLevel.Information => HtcSharp.Logging.Internal.LogLevel.Info,
                LogLevel.Warning => HtcSharp.Logging.Internal.LogLevel.Warn,
                LogLevel.Error => HtcSharp.Logging.Internal.LogLevel.Error,
                LogLevel.Critical => HtcSharp.Logging.Internal.LogLevel.Fatal,
                _ => HtcSharp.Logging.Internal.LogLevel.Debug
            };
        }
    }
}