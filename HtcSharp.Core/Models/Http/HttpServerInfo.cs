using HtcSharp.Core.Components.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtcSharp.Core.Managers;

namespace HtcSharp.Core.Models.Http {
    public class HttpServerInfo {
        public HttpServerInfo(IEnumerable<string> hosts, IEnumerable<string> domains, string root, bool useSsl, string certificate, string password, HttpLocationManager httpLocationManager, ErrorMessagesManager errorMessageManager) {
            var tempHosts = (from host in hosts select host.Split(":") into rawSplit let address = IPAddress.Parse(rawSplit[0]) let port = int.Parse(rawSplit[1]) select new IPEndPoint(address, port)).ToList();
            Hosts = tempHosts.AsReadOnly();
            Domains = domains.ToList().AsReadOnly();
            Root = root;
            UseSsl = useSsl;
            Certificate = certificate;
            Password = password;
            LocationManager = httpLocationManager;
            ErrorMessageManager = errorMessageManager;
        }

        public IReadOnlyCollection<IPEndPoint> Hosts { get; }
        public IReadOnlyCollection<string> Domains { get; }
        public string Root { get; }
        public bool UseSsl { get; }
        public string Certificate { get; }
        public string Password { get; }
        public HttpLocationManager LocationManager { get; }
        public ErrorMessagesManager ErrorMessageManager { get; }
    }
}
