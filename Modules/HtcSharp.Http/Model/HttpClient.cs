using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.Http.Model.Http;

namespace HtcSharp.Http.Model {
    public partial class HttpClient : IDisposable {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Socket _socket;
        private readonly Stream _stream;
        private readonly Encoder _encoder;
        private readonly Decoder _decoder;
        private readonly CancellationTokenSource _token;
        private HttpContext _httpContext;
        private HttpRequest _httpRequest;
        private HttpResponse _httpResponse;
        private HttpConnection _httpConnection;

        public HttpClient(Socket socket) {
            _socket = socket;
            _stream = new NetworkStream(_socket);
            _encoder = new Encoder(this);
            _decoder = new Decoder(this);
            _token = new CancellationTokenSource();
        }

        public async Task Start() {
            _httpConnection = new HttpConnection(
                _socket.LocalEndPoint as IPEndPoint, 
                _socket.RemoteEndPoint as IPEndPoint, 
                null);
            await _decoder.RunAsync();
        }

        public void Dispose() {
            _stream?.Dispose();
            _socket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
