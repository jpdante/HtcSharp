using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule2.Core.Infrastructure;
using HtcSharp.HttpModule2.Core.Logging;

namespace HtcSharp.HttpModule2.Core {
    public class HtcServer : IDisposable {
        internal static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        protected HtcServerOptions ServerOptions;
        protected ConnectionManager ConnectionManager;
        protected HeartbeatManager HeartbeatManager;
        protected PipeScheduler Scheduler;
        protected IHttpParser<Http1ParsingHandler> HttpParser;
        protected ISystemClock SystemClock;
        protected DateHeaderValueManager DateHeaderValueManager;
        protected Heartbeat Heartbeat;
        protected HtcLogger HtcLogger;

        public HtcServer(HtcServerOptions serverOptions) {
            ServerOptions = serverOptions;
            HtcLogger = new HtcLogger(Logger);
            ConnectionManager = new ConnectionManager(HtcLogger, ServerOptions.Limits.MaxConcurrentUpgradedConnections);
            HeartbeatManager = new HeartbeatManager(ConnectionManager);
            Heartbeat = new Heartbeat(new IHeartbeatHandler[] { dateHeaderValueManager, heartbeatManager }, new SystemClock(), DebuggerWrapper.Singleton, trace);
            HttpCharacters.Initialize();
        }

        public async Task StartAsync(CancellationToken cancellationToken) {

        }

        public async Task StopAsync(CancellationToken cancellationToken) {

        }

        public void Dispose() {
            var cancelledTokenSource = new CancellationTokenSource();
            cancelledTokenSource.Cancel();
            StopAsync(cancelledTokenSource.Token).GetAwaiter().GetResult();
        }
    }
}
