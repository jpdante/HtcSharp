// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IHttpBufferingFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    [Obsolete("See IHttpRequestBodyFeature or IHttpResponseBodyFeature DisableBuffering", error: true)]
    public interface IHttpBufferingFeature {
        void DisableRequestBuffering();
        void DisableResponseBuffering();
    }
}