using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Core.Http.Features {
    public interface IHttpWebSocketFeature {
        /// <summary>
        /// Indicates if this is a WebSocket upgrade request.
        /// </summary>
        bool IsWebSocketRequest { get; }

        /// <summary>
        /// Attempts to upgrade the request to a <see cref="WebSocket"/>. Check <see cref="IsWebSocketRequest"/>
        /// before invoking this.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<WebSocket> AcceptAsync(WebSocketAcceptContext context);
    }
}