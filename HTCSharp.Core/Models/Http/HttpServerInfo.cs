using HTCSharp.Core.Components.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HTCSharp.Core.Models.Http {
    public class HttpServerInfo {

        private IReadOnlyCollection<IPEndPoint> _Hosts;
        private string _Domain;
        private string _Root;
        private bool _UseSSL;
        private string _Certificate;
        private string _Password;
        private HttpRewriter _HttpRewriter;

        public HttpServerInfo(IReadOnlyCollection<string> hosts, string domain, string root, bool useSSL, string certificate, string password, HttpRewriter httpRewriter) {
            List<IPEndPoint> thosts = new List<IPEndPoint>();
            foreach(var host in hosts) {
                string[] rawSplit = host.Split(":");
                IPAddress address = IPAddress.Parse(rawSplit[0]);
                int port = int.Parse(rawSplit[1]);
                thosts.Add(new IPEndPoint(address, port));
            }
            _Hosts = thosts.AsReadOnly();
            _Domain = domain;
            _Root = root;
            _UseSSL = useSSL;
            _Certificate = certificate;
            _Password = password;
            _HttpRewriter = httpRewriter;
        }

        public IReadOnlyCollection<IPEndPoint> Hosts { get { return _Hosts; } }
        public string Domain { get { return _Domain; } }
        public string Root { get { return _Root; } }
        public bool UseSSL { get { return _UseSSL; } }
        public string Certificate { get { return _Certificate; } }
        public string Password { get { return _Password; } }
        public HttpRewriter GetHttpRewriter { get { return _HttpRewriter; } }
    }
}
