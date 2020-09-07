// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;

namespace HtcSharp.HttpModule.Connections.Abstractions.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Features\IConnectionEndPointFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface IConnectionEndPointFeature {
        EndPoint LocalEndPoint { get; set; }
        EndPoint RemoteEndPoint { get; set; }
    }
}