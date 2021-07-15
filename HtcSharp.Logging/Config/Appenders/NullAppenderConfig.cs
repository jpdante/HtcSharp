using HtcSharp.Logging.Appenders;

namespace HtcSharp.Logging.Config.Appenders {
    public class NullAppenderConfig : AppenderConfig {

        public override AppenderType Type { get; set; } = AppenderType.Null;

        public override IAppender GetAppender() {
            return new NullAppender();
        }
    }
}