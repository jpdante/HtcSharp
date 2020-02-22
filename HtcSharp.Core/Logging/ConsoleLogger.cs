using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Logging {
    public class ConsoleLogger : FormattedLogger {

        public LogLevel MinLogLevel { get; set; }

        public ConsoleLogger() : this(LogLevel.Trace) { }

        public ConsoleLogger(LogLevel minLogLevel) {
            MinLogLevel = minLogLevel;
        }

        public override void Log(LogLevel logLevel, object obj, Exception ex) {
            Console.WriteLine(FormatLog(logLevel, obj, ex));
        }

        public override bool IsEnabled(LogLevel logLevel) {
            return logLevel > MinLogLevel;
        }

        public override void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
