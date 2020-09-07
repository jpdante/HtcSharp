// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2Frame.Ping.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /* https://tools.ietf.org/html/rfc7540#section-6.7
        +---------------------------------------------------------------+
        |                                                               |
        |                      Opaque Data (64)                         |
        |                                                               |
        +---------------------------------------------------------------+
    */
    internal partial class Http2Frame {
        public Http2PingFrameFlags PingFlags {
            get => (Http2PingFrameFlags) Flags;
            set => Flags = (byte) value;
        }

        public bool PingAck => (PingFlags & Http2PingFrameFlags.ACK) == Http2PingFrameFlags.ACK;

        public void PreparePing(Http2PingFrameFlags flags) {
            PayloadLength = 8;
            Type = Http2FrameType.PING;
            PingFlags = flags;
            StreamId = 0;
        }
    }
}