using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HTCSharp.Core.Models.Http {
    public class HttpConnectionContext {

        private ConnectionInfo Connection;

        public X509Certificate ClientCertificate { get { return Connection.ClientCertificate; } }
        public string Id { get { return Connection.Id; } }
        public IPAddress LocalIpAddress { get { return Connection.LocalIpAddress; } }
        public int LocalPort { get { return Connection.LocalPort; } }
        public IPAddress RemoteIpAddress { get { return Connection.RemoteIpAddress; } }
        public int RemotePort { get { return Connection.RemotePort; } }

        public HttpConnectionContext(ConnectionInfo connection) {
            Connection = connection;
        }

    }
}
