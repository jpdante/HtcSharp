// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Abstractions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Abstractions\src\IHttpContextAccessor.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    public interface IHttpContextAccessor {
        HttpContext HttpContext { get; set; }
    }
}