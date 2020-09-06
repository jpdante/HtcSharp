// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Connections.Abstractions.Features;
using HtcSharp.HttpModule.Core.Features;
using HtcSharp.HttpModule.Http.Features.Interfaces;

namespace HtcSharp.HttpModule.Core.Internal {
    internal class TlsConnectionFeature : ITlsConnectionFeature, ITlsApplicationProtocolFeature, ITlsHandshakeFeature {
        public X509Certificate2 ClientCertificate { get; set; }

        public ReadOnlyMemory<byte> ApplicationProtocol { get; set; }

        public SslProtocols Protocol { get; set; }

        public CipherAlgorithmType CipherAlgorithm { get; set; }

        public int CipherStrength { get; set; }

        public HashAlgorithmType HashAlgorithm { get; set; }

        public int HashStrength { get; set; }

        public ExchangeAlgorithmType KeyExchangeAlgorithm { get; set; }

        public int KeyExchangeStrength { get; set; }

        public Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken) {
            return Task.FromResult(ClientCertificate);
        }
    }
}