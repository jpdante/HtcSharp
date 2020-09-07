// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Connections.Abstractions.Exceptions;

namespace HtcSharp.HttpModule.Core.Internal.Http {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http\IHttpOutputAborter.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal interface IHttpOutputAborter {
        void Abort(ConnectionAbortedException abortReason);
    }
}