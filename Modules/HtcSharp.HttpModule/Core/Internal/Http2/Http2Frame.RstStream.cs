// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2Frame.RstStream.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /* https://tools.ietf.org/html/rfc7540#section-6.4
        +---------------------------------------------------------------+
        |                        Error Code (32)                        |
        +---------------------------------------------------------------+
    */
    internal partial class Http2Frame {
        public Http2ErrorCode RstStreamErrorCode { get; set; }

        public void PrepareRstStream(int streamId, Http2ErrorCode errorCode) {
            PayloadLength = 4;
            Type = Http2FrameType.RST_STREAM;
            Flags = 0;
            StreamId = streamId;
            RstStreamErrorCode = errorCode;
        }
    }
}