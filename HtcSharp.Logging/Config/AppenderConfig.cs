using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Config.Appenders;

namespace HtcSharp.Logging.Config {
    public class AppenderConfig {

        public AppenderType Type { get; set; } = AppenderType.Null;

        public LogLevel LogLevel { get; set; }

        public FormatterConfig Formatter { get; set; } = null;

        public virtual IAppender GetAppender() {
            var formatter = Formatter?.GetFormatter();
            switch (Type) {
                case AppenderType.Multi:
                    return ((MultiAppenderConfig) this).GetAppender();
                case AppenderType.Console:
                    return new ConsoleAppender(LogLevel, formatter);
                case AppenderType.File:
                    return ((FileAppenderConfig) this).GetAppender();
                case AppenderType.Rolling:
                    return ((MultiAppenderConfig) this).GetAppender();
                default:
                    return new NullAppender();
            }
        }
    }
}