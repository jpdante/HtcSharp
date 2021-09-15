using System;

namespace HtcSharp.Logging {
    public interface IFormatter {

        public string FormatLog(ILogger logger, LogLevel logLevel, object msg, Exception ex, params object[] objs);

    }
}