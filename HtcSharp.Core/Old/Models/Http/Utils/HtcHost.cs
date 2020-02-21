namespace HtcSharp.Core.Old.Models.Http.Utils {
    public class HtcHost {
        private HostString _hostString;

        public string Host { get; }
        public int? Port { get; }
        public string Value { get; }
        public string UriComponent => _hostString.ToUriComponent();

        public HtcHost(HostString hostString) {
            _hostString = hostString;
            Host = _hostString.Host;
            Port = _hostString.Port;
            Value = _hostString.Value;
        }

    }
}
