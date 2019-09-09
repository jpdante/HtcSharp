using System.Net;
using System.Net.Sockets;
using System.Reflection;
using HtcSharp.Core.Engines;
using HtcSharp.Core.Logging;
using HtcSharp.Http.Manager;
using HtcSharp.Http.Model;
using HtcSharp.Http.Net;

namespace HtcSharp.Http {
    public class HttpEngine2 : Engine {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ListenerManager _listenerManager;

        public HttpEngine2() {
            _listenerManager = new ListenerManager();
            _listenerManager.CreateListener(new IPEndPoint(IPAddress.Any, 8080));
        }

        private async void OnReceiveSocketHandler(SocketListener socketlistener, Socket socket) {
            Logger.Info($"{((IPEndPoint) socket.RemoteEndPoint).Address}:{((IPEndPoint) socket.RemoteEndPoint).Port}");
            var httpClient = new HttpClient(socket);
            await httpClient.Start();
        }

        public override void Start() {
            _listenerManager.StartAllListeners();
            foreach (var listener in _listenerManager.GetListeners()) {
                //var j = await listener.AcceptAsync();
            }
        }

        public override void Stop() {
            _listenerManager.StopAllListeners();
        }
    }
}