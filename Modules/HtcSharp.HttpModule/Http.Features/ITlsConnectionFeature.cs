// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\ITlsConnectionFeature.cs
    // Start-At-Remote-Line 9
    // SourceTools-End
    public interface ITlsConnectionFeature {
        /// <summary>
        /// Synchronously retrieves the client certificate, if any.
        /// </summary>
        X509Certificate2 ClientCertificate { get; set; }

        /// <summary>
        /// Asynchronously retrieves the client certificate, if any.
        /// </summary>
        /// <returns></returns>
        Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken);
    }
}