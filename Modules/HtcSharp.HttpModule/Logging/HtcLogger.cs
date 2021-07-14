using System;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Logging {
    public class HtcLogger : ILogger {

        private readonly HtcSharp.Logging.ILogger _logger;

        public HtcLogger(HtcSharp.Logging.ILogger logger) {
            _logger = logger;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            string eventName = string.IsNullOrEmpty(eventId.Name) ? null : $" {eventId.Name}";
            _logger.Log(ConvertLogLevel(logLevel), $"[EID {eventId.Id}{eventName}] {state}", exception);
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        private static HtcSharp.Logging.LogLevel ConvertLogLevel(LogLevel logLevel) {
            return logLevel switch {
                LogLevel.Trace => HtcSharp.Logging.LogLevel.Debug,
                LogLevel.Debug => HtcSharp.Logging.LogLevel.Debug,
                LogLevel.None => HtcSharp.Logging.LogLevel.Debug,
                LogLevel.Information => HtcSharp.Logging.LogLevel.Info,
                LogLevel.Warning => HtcSharp.Logging.LogLevel.Warn,
                LogLevel.Error => HtcSharp.Logging.LogLevel.Error,
                LogLevel.Critical => HtcSharp.Logging.LogLevel.Fatal,
                _ => HtcSharp.Logging.LogLevel.Debug
            };
        }
    }
}