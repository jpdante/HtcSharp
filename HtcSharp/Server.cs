using System;
using System.Threading.Tasks;
using HtcSharp.Internal;
using HtcSharp.Logging;
using HtcSharp.Logging.Appenders;
using HtcSharp.Logging.Internal;

namespace HtcSharp {
    public class Server : DaemonApplication {

        private readonly ILogger Logger = LoggerManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        protected override Task OnLoad() {
            /*var multiAppender = new MultiAppender();
            multiAppender.AddAppender(new ConsoleAppender(LogLevel.All));
            multiAppender.AddAppender(new RollingFileAppender(new RollingFileAppender.RollingFileConfig(), LogLevel.All));*/
            LoggerManager.Init(new RollingFileAppender(new RollingFileAppender.RollingFileConfig {
                RollingSpan = new TimeSpan(0, 0, 0, 1)
            }, LogLevel.All));
            Logger.LogInfo("Loading...");
            return Task.CompletedTask;
        }

        protected override Task OnStart() {
            Logger.LogInfo("Starting...");
            Run();
            return Task.CompletedTask;
        }

        public async Task Run() {
            Logger.LogInfo(Guid.NewGuid().ToString());
            await Task.Delay(500);
            Run();
        }

        protected override Task OnExit() {
            Logger.LogInfo("Exiting...");
            return Task.CompletedTask;
        }

    }
}