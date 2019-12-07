using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule2.Core {
    public class HtcServer : IDisposable {

        public HtcServer() {

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
