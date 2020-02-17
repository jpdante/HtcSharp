// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule2.Http.Protocols.Http2
{
    internal readonly struct Http2PeerSetting
    {
        public Http2PeerSetting(Http2SettingsParameter parameter, uint value)
        {
            Parameter = parameter;
            Value = value;
        }

        public Http2SettingsParameter Parameter { get; }

        public uint Value { get; }
    }
}