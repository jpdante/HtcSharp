// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Internal.Http {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http\HttpRequestTargetForm.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal enum HttpRequestTarget {
        Unknown = -1,

        // origin-form is the most common
        OriginForm,
        AbsoluteForm,
        AuthorityForm,
        AsteriskForm
    }
}