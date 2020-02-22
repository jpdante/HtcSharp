using System;
using System.Collections.Generic;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Logging {
    public class MemoryLogger : ILogger {

        public readonly List<LogEntry> Logs;

        public MemoryLogger() {
            Logs = new List<LogEntry>();
        }

        public void Log(LogLevel logLevel, object obj, Exception ex) {
            Logs.Add(new LogEntry(logLevel, obj, ex));
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public void Dispose() {
            Logs.Clear();
            GC.SuppressFinalize(this);
        }

        public class LogEntry {

            public readonly DateTime DateTime;
            public readonly LogLevel LogLevel;
            public readonly object Object;
            public readonly Exception Exception;

            public LogEntry(LogLevel logLevel, object obj, Exception ex) {
                DateTime = DateTime.Now;
                LogLevel = logLevel;
                Object = obj;
                Exception = ex;
            }
        }
    }
}