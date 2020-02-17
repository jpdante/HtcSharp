// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule2.Http.Features
{
    public class TlsConnectionFeature : Microsoft.AspNetCore.Http.Features.ITlsConnectionFeature
    {
        public X509Certificate2 ClientCertificate { get; set; }

        public Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ClientCertificate);
        }
    }
}