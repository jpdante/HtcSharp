using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Loggers {
    public class MultiLogger : ILogger {

        private readonly List<ILogger> _loggers;

        public MultiLogger() {
            _loggers = new List<ILogger>();
        }

        public void Log(LogLevel logLevel, string msg, params object[] objs) {
            foreach (var logger in _loggers) {
                try {
                    logger.Log(logLevel, msg, objs);
                } catch {
                    // ignored
                }
            }
        }

        public void Log(LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            foreach (var logger in _loggers) {
                try {
                    logger.Log(logLevel, msg, ex, objs);
                } catch {
                    // ignored
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void AddLogger(ILogger logger) {
            _loggers.Add(logger);
        }

        public void RemoveLogger(ILogger logger) {
            _loggers.Remove(logger);
        }

        public void ClearLoggers() {
            _loggers.Clear();
        }

        public IReadOnlyList<ILogger> Loggers => _loggers;

    }
}