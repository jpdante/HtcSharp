using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCHost {

        private HostString HostString;

        public string Host { get; }
        public int? Port { get; }
        public string Value { get; }
        public string UriComponent { get { return HostString.ToUriComponent(); } }

        public HTCHost(HostString hostString) {
            HostString = hostString;
            Host = HostString.Host;
            Port = HostString.Port;
            Value = HostString.Value;
        }

    }
}
