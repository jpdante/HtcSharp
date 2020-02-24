using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Logging.Abstractions {
    public interface ILogger : IDisposable {

        void Log(LogLevel logLevel, object obj, Exception ex);
        bool IsEnabled(LogLevel logLevel);

    }
}
