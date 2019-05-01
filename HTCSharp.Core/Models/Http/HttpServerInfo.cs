using HtcSharp.Core.Components.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HtcSharp.Core.Models.Http {
    public class HttpServerInfo {
        public HttpServerInfo(IEnumerable<string> hosts, string domain, string root, bool useSsl, string certificate, string password, HttpReWriter httpReWriter) {
            var tempHosts = (from host in hosts select host.Split(":") into rawSplit let address = IPAddress.Parse(rawSplit[0]) let port = int.Parse(rawSplit[1]) select new IPEndPoint(address, port)).ToList();
            Hosts = tempHosts.AsReadOnly();
            Domain = domain;
            Root = root;
            UseSsl = useSsl;
            Certificate = certificate;
            Password = password;
            GetHttpReWriter = httpReWriter;
        }

        public IReadOnlyCollection<IPEndPoint> Hosts { get; }
        public string Domain { get; }
        public string Root { get; }
        public bool UseSsl { get; }
        public string Certificate { get; }
        public string Password { get; }
        public HttpReWriter GetHttpReWriter { get; }
    }
}
