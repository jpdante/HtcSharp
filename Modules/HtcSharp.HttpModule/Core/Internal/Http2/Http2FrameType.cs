// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2FrameType.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal enum Http2FrameType : byte {
        DATA = 0x0,
        HEADERS = 0x1,
        PRIORITY = 0x2,
        RST_STREAM = 0x3,
        SETTINGS = 0x4,
        PUSH_PROMISE = 0x5,
        PING = 0x6,
        GOAWAY = 0x7,
        WINDOW_UPDATE = 0x8,
        CONTINUATION = 0x9
    }
}