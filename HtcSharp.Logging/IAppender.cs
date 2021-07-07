using System;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging {
    public interface IAppender : IDisposable {
        
        void Log(ILogger logger, LogLevel logLevel, string msg, params object[] objs);
        void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs);
        bool IsEnabled(LogLevel logLevel);

    }
}