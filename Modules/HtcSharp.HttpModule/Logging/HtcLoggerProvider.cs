using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Logging {
    public class HtcLoggerProvider : ILoggerProvider {

        private readonly HtcSharp.Logging.ILogger _logger;

        public HtcLoggerProvider(HtcSharp.Logging.ILogger logger) {
            _logger = logger;
        }

        public ILogger CreateLogger(string categoryName) {
            return new HtcLogger(_logger);
        }

        public void Dispose() {

        }
    }
}