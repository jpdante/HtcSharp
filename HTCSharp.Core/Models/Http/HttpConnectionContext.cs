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
        public string ID { get { return Connection.Id; } }
        public IPAddress LocalIPAddress { get { return Connection.LocalIpAddress; } }
        public int LocalPort { get { return Connection.LocalPort; } }
        public IPAddress RemoteIPAddress { get { return Connection.RemoteIpAddress; } }
        public int RemotePort { get { return Connection.RemotePort; } }

        public HttpConnectionContext(ConnectionInfo connection) {
            Connection = connection;
        }

    }
}
