using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Config.Appenders;

namespace HtcSharp.Logging.Config {
    public abstract class AppenderConfig {

        public abstract AppenderType Type { get; set; }

        public LogLevel LogLevel { get; set; }

        public FormatterConfig Formatter { get; set; } = null;

        public virtual IAppender GetAppender() {
            var formatter = Formatter?.GetFormatter();
            switch (Type) {
                case AppenderType.Null:
                    return ((NullAppenderConfig) this).GetAppender();
                case AppenderType.Multi:
                    return ((MultiAppenderConfig) this).GetAppender();
                case AppenderType.Console:
                    return ((ConsoleAppenderConfig) this).GetAppender();
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