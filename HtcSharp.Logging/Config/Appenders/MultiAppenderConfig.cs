using System.Collections.Generic;
using HtcSharp.Logging.Appenders;

namespace HtcSharp.Logging.Config.Appenders {
    public class MultiAppenderConfig : AppenderConfig {
        public List<AppenderConfig> Appenders { get; set; } = new List<AppenderConfig>();

        public override IAppender GetAppender() {
            var multiAppender = new MultiAppender();
            var formatter = Formatter?.GetFormatter();
            if (Appenders == null) return multiAppender;
            foreach (var appenderConfig in Appenders) {
                switch (Type) {
                    case AppenderType.Multi:
                        return ((MultiAppenderConfig) appenderConfig).GetAppender();
                    case AppenderType.Console:
                        return new ConsoleAppender(LogLevel, formatter);
                    case AppenderType.File:
                        return ((FileAppenderConfig) appenderConfig).GetAppender();
                    case AppenderType.Rolling:
                        return ((MultiAppenderConfig) appenderConfig).GetAppender();
                    default:
                        return new NullAppender();
                }
            }
            return multiAppender;
        }
    }
}