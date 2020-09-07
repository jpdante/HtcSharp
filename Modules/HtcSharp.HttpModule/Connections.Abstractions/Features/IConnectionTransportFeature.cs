// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO.Pipelines;

namespace HtcSharp.HttpModule.Connections.Abstractions.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Features\IConnectionTransportFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface IConnectionTransportFeature {
        IDuplexPipe Transport { get; set; }
    }
}