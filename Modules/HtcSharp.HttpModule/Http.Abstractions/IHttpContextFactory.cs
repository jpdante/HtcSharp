// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Http.Features;

namespace HtcSharp.HttpModule.Http.Abstractions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Abstractions\src\IHttpContextFactory.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface IHttpContextFactory {
        HttpContext Create(IFeatureCollection featureCollection);
        void Dispose(HttpContext httpContext);
    }
}