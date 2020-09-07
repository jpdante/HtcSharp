// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Shared.CertificateGeneration {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Shared\CertificateGeneration\EnsureCertificateResult.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal enum EnsureCertificateResult {
        Succeeded = 1,
        ValidCertificatePresent,
        ErrorCreatingTheCertificate,
        ErrorSavingTheCertificateIntoTheCurrentUserPersonalStore,
        ErrorExportingTheCertificate,
        FailedToTrustTheCertificate,
        UserCancelledTrustStep,
        FailedToMakeKeyAccessible,
    }
}