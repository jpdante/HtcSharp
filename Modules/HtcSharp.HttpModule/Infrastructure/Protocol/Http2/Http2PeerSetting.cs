namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    internal readonly struct Http2PeerSetting {
        public Http2PeerSetting(Http2SettingsParameter parameter, uint value) {
            Parameter = parameter;
            Value = value;
        }

        public Http2SettingsParameter Parameter { get; }

        public uint Value { get; }
    }
}
