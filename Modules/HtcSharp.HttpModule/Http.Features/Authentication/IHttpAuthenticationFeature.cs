// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;

namespace HtcSharp.HttpModule.Http.Features.Authentication {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\Authentication\IHttpAuthenticationFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface IHttpAuthenticationFeature {
        ClaimsPrincipal User { get; set; }
    }
}