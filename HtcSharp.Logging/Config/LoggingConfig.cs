using System.Collections.Generic;
using System.Threading;
using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Config.Appenders;

namespace HtcSharp.Logging.Config {
    public class LoggingConfig {
        public AppenderConfig Appender { get; set; } = new MultiAppenderConfig {
            Type = AppenderType.Multi
        };

        public IAppender GetAppender() {
            return Appender == null ? new NullAppender() : Appender.GetAppender();
        }
    }
}