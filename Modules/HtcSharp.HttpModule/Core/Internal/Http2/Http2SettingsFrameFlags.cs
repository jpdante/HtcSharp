// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2SettingsFrameFlags.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    [Flags]
    internal enum Http2SettingsFrameFlags : byte {
        NONE = 0x0,
        ACK = 0x1,
    }
}