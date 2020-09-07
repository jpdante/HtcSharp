// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http\src\Features\DefaultSessionFeature.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /// <summary>
    /// This type exists only for the purpose of unit testing where the user can directly set the
    /// <see cref="HttpContext.Session"/> property without the need for creating a <see cref="ISessionFeature"/>.
    /// </summary>
    public class DefaultSessionFeature : ISessionFeature {
        public ISession Session { get; set; }
    }
}