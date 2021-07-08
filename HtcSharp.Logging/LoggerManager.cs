using System;
using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging {
    public class LoggerManager {

        internal static IAppender DefaultAppender;

        static LoggerManager() {
            DefaultAppender = new NullAppender();
        }

        public static void Init(IAppender appender) {
            DefaultAppender = appender;
        }

        public static ILogger GetLogger(Type type) {
            return new Logger(type);
        }
    }
}
