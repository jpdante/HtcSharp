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

        public HttpEngine() {
            _listenerManager = new ListenerManager();
            _listenerManager.CreateListener(new IPEndPoint(IPAddress.Any, 80));
        }

        private async void OnReceiveSocketHandler(SocketListener socketlistener, Socket socket) {
            Logger.Info($"{((IPEndPoint) socket.RemoteEndPoint).Address}:{((IPEndPoint) socket.RemoteEndPoint).Port}");
            var httpClient = new HttpClient(socket);
            await httpClient.Start();
        }

        public override async void Start() {
            _listenerManager.StartAllListeners();
            foreach (var listener in _listenerManager.GetListeners()) {
                var j = await listener.AcceptAsync();
                await Task.Factory.StartNew(async () => {
                    Logger.Info($"New Connection: {((IPEndPoint) j.RemoteEndPoint).Address}");
                    var httpClient = new HttpClient(j);
                    await httpClient.Start();
                    Logger.Info($"Connection processed!");
                });
            }
        }

        public override void Stop() {
            _listenerManager.StopAllListeners();
        }
    }
}