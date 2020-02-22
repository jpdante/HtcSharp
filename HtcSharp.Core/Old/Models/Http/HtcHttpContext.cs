using System.Collections.Generic;
using System.Net;

namespace HtcSharp.Core.Old.Models.Http {
    public class HtcHttpContext {
        public HttpRequestContext Request { get; }
        public HttpResponseContext Response { get; }
        public HttpConnectionContext Connection { get; }
        public BasicServerInfo ServerInfo { get; }
        public ErrorMessagesManager ErrorMessageManager { get; }

        public HtcHttpContext(HttpContext context, HttpServerInfo serverInfo) {
            Request = new HttpRequestContext(context.Request);
            Response = new HttpResponseContext(context.Response);
            Connection = new HttpConnectionContext(context.Connection);
            ServerInfo = new BasicServerInfo(serverInfo);
            ErrorMessageManager = serverInfo.ErrorMessageManager;
        }
    }

    public class BasicServerInfo {
        public readonly string RootPath;
        public readonly IReadOnlyCollection<string> Domain;
        public readonly bool UseSsl;
        public readonly IReadOnlyCollection<IPEndPoint> Hosts;

        public BasicServerInfo(HttpServerInfo serverInfo) {
            RootPath = serverInfo.Root;
            Domain = serverInfo.Domains;
            UseSsl = serverInfo.UseSsl;
            Hosts = serverInfo.Hosts;
        }
    }
}
