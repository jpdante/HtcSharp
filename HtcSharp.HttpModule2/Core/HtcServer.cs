using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule2.Core.Infrastructure;

namespace HtcSharp.HttpModule2.Core {
    public class HtcServer : IDisposable {
        internal static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private HtcServerConfiguration _configuration;

        public HtcServer(HtcServerConfiguration serverConfiguration) {
            _configuration = serverConfiguration;
            var heartbeatManager = new HeartbeatManager(connectionManager);
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
