using System;
using System.Collections.Generic;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Logging {
    public class MultiLogger : ILogger {

        private readonly Dictionary<string, ILogger> _loggers;

        public MultiLogger() {
            _loggers = new Dictionary<string, ILogger>();
        }

        public void AddLogger(string name, ILogger logger) {
            _loggers.Add(name, logger);
        }

        public void RemoveLogger(string name) {
            _loggers.Remove(name);
        }

        public void Log(LogLevel logLevel, Type type, object obj, Exception ex) {
            foreach(var logger in _loggers.Values) {
                logger.Log(logLevel, type, obj, ex);
            }
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public void Dispose() {
            foreach (var logger in _loggers.Values) {
                logger.Dispose();
            }
            _loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}