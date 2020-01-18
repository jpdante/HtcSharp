﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.HttpModule.Core.Http.Http2 {
    /* https://tools.ietf.org/html/rfc7540#section-6.5.1
        List of:
        +-------------------------------+
        |       Identifier (16)         |
        +-------------------------------+-------------------------------+
        |                        Value (32)                             |
        +---------------------------------------------------------------+
    */
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
