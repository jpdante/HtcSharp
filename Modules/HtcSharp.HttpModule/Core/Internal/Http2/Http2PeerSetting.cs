// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2PeerSetting.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal readonly struct Http2PeerSetting {
        public Http2PeerSetting(Http2SettingsParameter parameter, uint value) {
            Parameter = parameter;
            Value = value;
        }

        public Http2SettingsParameter Parameter { get; }

        public uint Value { get; }
    }
}