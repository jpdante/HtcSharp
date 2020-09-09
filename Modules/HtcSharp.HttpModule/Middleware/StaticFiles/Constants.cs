// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Middleware.StaticFiles {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Middleware\StaticFiles\src\Constants.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal static class Constants {
        internal const string ServerCapabilitiesKey = "server.Capabilities";
        internal const string SendFileVersionKey = "sendfile.Version";
        internal const string SendFileVersion = "1.0";

        internal const int Status200Ok = 200;
        internal const int Status206PartialContent = 206;
        internal const int Status304NotModified = 304;
        internal const int Status412PreconditionFailed = 412;
        internal const int Status416RangeNotSatisfiable = 416;
    }
}
