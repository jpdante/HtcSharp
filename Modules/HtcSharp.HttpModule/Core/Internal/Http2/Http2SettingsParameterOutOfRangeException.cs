// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2SettingsParameterOutOfRangeException.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal sealed class Http2SettingsParameterOutOfRangeException : Exception {
        public Http2SettingsParameterOutOfRangeException(Http2SettingsParameter parameter, long lowerBound, long upperBound)
            : base($"HTTP/2 SETTINGS parameter {parameter} must be set to a value between {lowerBound} and {upperBound}") {
            Parameter = parameter;
        }

        public Http2SettingsParameter Parameter { get; }
    }
}