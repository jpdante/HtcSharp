// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\ClientCertificateMode.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /// <summary>
    /// Describes the client certificate requirements for a HTTPS connection.
    /// </summary>
    public enum ClientCertificateMode {
        /// <summary>
        /// A client certificate is not required and will not be requested from clients.
        /// </summary>
        NoCertificate,

        /// <summary>
        /// A client certificate will be requested; however, authentication will not fail if a certificate is not provided by the client.
        /// </summary>
        AllowCertificate,

        /// <summary>
        /// A client certificate will be requested, and the client must provide a valid certificate for authentication to succeed.
        /// </summary>
        RequireCertificate
    }
}