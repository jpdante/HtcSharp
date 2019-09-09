using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HtcSharp.Http.Model.Http {
    public class HttpConnection {
        public IPEndPoint LocalEndPoint;
        public IPEndPoint RemoteEndPoint;
        public X509Certificate2 ClientCertificate;

        public HttpConnection(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, X509Certificate2 clientCertificate) {
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
            ClientCertificate = clientCertificate;
        }
    }
}
