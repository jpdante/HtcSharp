using HtcSharp.Logging.Appenders;

namespace HtcSharp.Logging.Config.Appenders {
    public class ConsoleAppenderConfig : AppenderConfig {

        public override AppenderType Type { get; set; } = AppenderType.Console;

        public override IAppender GetAppender() {
            var formatter = Formatter?.GetFormatter();
            return new ConsoleAppender(LogLevel, formatter);
        }
    }
}