using System.Collections.Generic;

namespace HtcSharp.HttpModule2.Core.Http2 {
    internal class Http2PeerSettings {
        public const uint DefaultHeaderTableSize = 4096;
        public const bool DefaultEnablePush = true;
        public const uint DefaultMaxConcurrentStreams = uint.MaxValue;
        public const uint DefaultInitialWindowSize = 65535;
        public const uint DefaultMaxFrameSize = MinAllowedMaxFrameSize;
        public const uint DefaultMaxHeaderListSize = uint.MaxValue;
        public const uint MaxWindowSize = int.MaxValue;
        internal const int MinAllowedMaxFrameSize = 16 * 1024;
        internal const int MaxAllowedMaxFrameSize = 16 * 1024 * 1024 - 1;

        public uint HeaderTableSize { get; set; } = DefaultHeaderTableSize;

        public bool EnablePush { get; set; } = DefaultEnablePush;

        public uint MaxConcurrentStreams { get; set; } = DefaultMaxConcurrentStreams;

        public uint InitialWindowSize { get; set; } = DefaultInitialWindowSize;

        public uint MaxFrameSize { get; set; } = DefaultMaxFrameSize;

        public uint MaxHeaderListSize { get; set; } = DefaultMaxHeaderListSize;

        // TODO: Return the diff so we can react
        public void Update(IList<Http2PeerSetting> settings) {
            foreach (var setting in settings) {
                var value = setting.Value;

                switch (setting.Parameter) {
                    case Http2SettingsParameter.SETTINGS_HEADER_TABLE_SIZE:
                        HeaderTableSize = value;
                        break;
                    case Http2SettingsParameter.SETTINGS_ENABLE_PUSH:
                        if (value != 0 && value != 1)
                            throw new Http2SettingsParameterOutOfRangeException(
                                Http2SettingsParameter.SETTINGS_ENABLE_PUSH,
                                0,
                                1);

                        EnablePush = value == 1;
                        break;
                    case Http2SettingsParameter.SETTINGS_MAX_CONCURRENT_STREAMS:
                        MaxConcurrentStreams = value;
                        break;
                    case Http2SettingsParameter.SETTINGS_INITIAL_WINDOW_SIZE:
                        if (value > MaxWindowSize)
                            throw new Http2SettingsParameterOutOfRangeException(
                                Http2SettingsParameter.SETTINGS_INITIAL_WINDOW_SIZE,
                                0,
                                MaxWindowSize);

                        InitialWindowSize = value;
                        break;
                    case Http2SettingsParameter.SETTINGS_MAX_FRAME_SIZE:
                        if (value < MinAllowedMaxFrameSize || value > MaxAllowedMaxFrameSize)
                            throw new Http2SettingsParameterOutOfRangeException(
                                Http2SettingsParameter.SETTINGS_MAX_FRAME_SIZE,
                                MinAllowedMaxFrameSize,
                                MaxAllowedMaxFrameSize);

                        MaxFrameSize = value;
                        break;
                    case Http2SettingsParameter.SETTINGS_MAX_HEADER_LIST_SIZE:
                        MaxHeaderListSize = value;
                        break;
                }
            }
        }

        internal IList<Http2PeerSetting> GetNonProtocolDefaults() {
            var list = new List<Http2PeerSetting>(1);
            if (HeaderTableSize != DefaultHeaderTableSize)
                list.Add(new Http2PeerSetting(Http2SettingsParameter.SETTINGS_HEADER_TABLE_SIZE, HeaderTableSize));
            if (EnablePush != DefaultEnablePush)
                list.Add(new Http2PeerSetting(Http2SettingsParameter.SETTINGS_ENABLE_PUSH, EnablePush ? 1u : 0));
            if (MaxConcurrentStreams != DefaultMaxConcurrentStreams)
                list.Add(new Http2PeerSetting(Http2SettingsParameter.SETTINGS_MAX_CONCURRENT_STREAMS,
                    MaxConcurrentStreams));
            if (InitialWindowSize != DefaultInitialWindowSize)
                list.Add(new Http2PeerSetting(Http2SettingsParameter.SETTINGS_INITIAL_WINDOW_SIZE, InitialWindowSize));
            if (MaxFrameSize != DefaultMaxFrameSize)
                list.Add(new Http2PeerSetting(Http2SettingsParameter.SETTINGS_MAX_FRAME_SIZE, MaxFrameSize));
            if (MaxHeaderListSize != DefaultMaxHeaderListSize)
                list.Add(new Http2PeerSetting(Http2SettingsParameter.SETTINGS_MAX_HEADER_LIST_SIZE, MaxHeaderListSize));
            return list;
        }
    }
}