using System.Threading.Tasks;
using HtcSharp.Internal;
using HtcSharp.Logging;
using HtcSharp.Logging.Internal;

namespace HtcSharp {
    public class Server : DaemonApplication {

        private MultiLogger _logger;

        protected override Task OnLoad() {
            _logger = new MultiLogger();
            _logger.AddLogger(new ConsoleLogger(LogLevel.All));
            return Task.CompletedTask;
        }

        protected override Task OnStart() {
            _logger.LogFatal("TOP!!");
            return Task.CompletedTask;
        }

        protected override Task OnExit() {
            return Task.CompletedTask;
        }

    }
}