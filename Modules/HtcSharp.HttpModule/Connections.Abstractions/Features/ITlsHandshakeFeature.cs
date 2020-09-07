// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Authentication;

namespace HtcSharp.HttpModule.Connections.Abstractions.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Features\ITlsHandshakeFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface ITlsHandshakeFeature {
        SslProtocols Protocol { get; }

        CipherAlgorithmType CipherAlgorithm { get; }

        int CipherStrength { get; }

        HashAlgorithmType HashAlgorithm { get; }

        int HashStrength { get; }

        ExchangeAlgorithmType KeyExchangeAlgorithm { get; }

        int KeyExchangeStrength { get; }
    }
}