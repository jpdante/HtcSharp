using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HtcSharp.HttpModule.Http.Abstractions {
    public class HtcServerInfo {

        public string RootPath { get; internal set; }
        public IReadOnlyCollection<string> Domain { get; internal set; }
        public bool UseSsl { get; internal set; }
        public IReadOnlyCollection<IPEndPoint> Hosts { get; internal set; }

    }
}
