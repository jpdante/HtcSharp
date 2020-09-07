// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2Frame.GoAway.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /* https://tools.ietf.org/html/rfc7540#section-6.8
        +-+-------------------------------------------------------------+
        |R|                  Last-Stream-ID (31)                        |
        +-+-------------------------------------------------------------+
        |                      Error Code (32)                          |
        +---------------------------------------------------------------+
        |                  Additional Debug Data (*)                    |
        +---------------------------------------------------------------+
    */
    internal partial class Http2Frame {
        public int GoAwayLastStreamId { get; set; }

        public Http2ErrorCode GoAwayErrorCode { get; set; }

        public void PrepareGoAway(int lastStreamId, Http2ErrorCode errorCode) {
            PayloadLength = 8;
            Type = Http2FrameType.GOAWAY;
            Flags = 0;
            StreamId = 0;
            GoAwayLastStreamId = lastStreamId;
            GoAwayErrorCode = errorCode;
        }
    }
}