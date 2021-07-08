using System.Threading.Tasks;
using HtcSharp.Internal;
using HtcSharp.Logging;
using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Internal;

namespace HtcSharp {
    public class Server : DaemonApplication {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        protected override Task OnLoad() {
            var multiAppender = new MultiAppender();
            multiAppender.AddAppender(new ConsoleAppender(LogLevel.All));
            multiAppender.AddAppender(new RollingFileAppender(new RollingFileAppender.RollingFileConfig(), LogLevel.All));
            LoggerManager.Init(multiAppender);
            return Task.CompletedTask;
        }

        protected override Task OnStart() {
            Logger.LogFatal("TOP!!");
            return Task.CompletedTask;
        }

        protected override Task OnExit() {
            return Task.CompletedTask;
        }

    }
}