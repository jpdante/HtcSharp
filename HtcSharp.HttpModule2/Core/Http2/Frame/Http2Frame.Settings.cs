using HtcSharp.HttpModule2.Core.Http2.Flags;

namespace HtcSharp.HttpModule2.Core.Http2.Frame {
    internal partial class Http2Frame {
        public Http2SettingsFrameFlags SettingsFlags {
            get => (Http2SettingsFrameFlags)Flags;
            set => Flags = (byte)value;
        }

        public bool SettingsAck => (SettingsFlags & Http2SettingsFrameFlags.ACK) == Http2SettingsFrameFlags.ACK;

        public void PrepareSettings(Http2SettingsFrameFlags flags) {
            PayloadLength = 0;
            Type = Http2FrameType.SETTINGS;
            SettingsFlags = flags;
            StreamId = 0;
        }
    }
}