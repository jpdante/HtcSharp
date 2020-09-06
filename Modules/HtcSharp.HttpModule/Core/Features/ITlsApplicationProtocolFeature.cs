// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Core.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Features\ITlsApplicationProtocolFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface ITlsApplicationProtocolFeature {
        ReadOnlyMemory<byte> ApplicationProtocol { get; }
    }
}