// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\IHttp2StreamLifetimeHandler.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal interface IHttp2StreamLifetimeHandler {
        void OnStreamCompleted(Http2Stream stream);
        void DecrementActiveClientStreamCount();
    }
}