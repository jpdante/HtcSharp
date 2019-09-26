using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Core.Engines;
using HtcSharp.Core.Logging;
using HtcSharp.Http.Manager;
using HtcSharp.Http.Model;
using HtcSharp.Http.Net;

namespace HtcSharp.Http {
    public class HttpEngine : Engine {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ListenerManager _listenerManager;

        Stopwatch stopWatch = new Stopwatch();

        public HttpEngine() {
            _listenerManager = new ListenerManager();
            _listenerManager.CreateListener(new IPEndPoint(IPAddress.Any, 80));
        }

        private void OnReceiveSocketHandler(SocketListener socketlistener, Socket socket) {
            Logger.Info($"{((IPEndPoint) socket.RemoteEndPoint).Address}:{((IPEndPoint) socket.RemoteEndPoint).Port}");
        }

        public override void Start() {
            _listenerManager.StartAllListeners();
            foreach (var listener in _listenerManager.GetListeners()) {
                WaitConnections(listener);
            }
        }

        private async void WaitConnections(SocketListener socketListener) {
            while (socketListener.IsListening) {
                Logger.Info("Listening for connection");
                var socket = await socketListener.AcceptAsync();

                _ = Task.Run(async () => {
                    Logger.Info($"New Connection: {((IPEndPoint)socket.RemoteEndPoint).Address}");
                    stopWatch.Start();
                    var httpClient = new HttpClient(socket);
                    await httpClient.Start();
                    stopWatch.Stop();
                    Logger.Info($"Decoder took: {stopWatch.ElapsedMilliseconds}ms");
                    stopWatch.Reset();
                });
            }
        }

        public override void Stop() {
            _listenerManager.StopAllListeners();
        }
    }
}