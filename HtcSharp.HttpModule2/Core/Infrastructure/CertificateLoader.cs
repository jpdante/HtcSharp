using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    public static class CertificateLoader {
        private const string ServerAuthenticationOid = "1.3.6.1.5.5.7.3.1";

        public static X509Certificate2 LoadFromStoreCert(string subject, string storeName, StoreLocation storeLocation, bool allowInvalid) {
            using (var store = new X509Store(storeName, storeLocation)) {
                X509Certificate2Collection storeCertificates = null;
                X509Certificate2 foundCertificate = null;

                try {
                    store.Open(OpenFlags.ReadOnly);
                    storeCertificates = store.Certificates;
                    var foundCertificates = storeCertificates.Find(X509FindType.FindBySubjectName, subject, !allowInvalid);
                    foundCertificate = foundCertificates
                        .OfType<X509Certificate2>()
                        .Where(IsCertificateAllowedForServerAuth)
                        .Where(DoesCertificateHaveAnAccessiblePrivateKey)
                        .OrderByDescending(certificate => certificate.NotAfter)
                        .FirstOrDefault();

                    if (foundCertificate == null) {
                        throw new InvalidOperationException(CoreStrings.FormatCertNotFoundInStore(subject, storeLocation, storeName, allowInvalid));
                    }

                    return foundCertificate;
                } finally {
                    DisposeCertificates(storeCertificates, except: foundCertificate);
                }
            }
        }

        internal static bool IsCertificateAllowedForServerAuth(X509Certificate2 certificate) {
            var hasEkuExtension = false;

            foreach (var extension in certificate.Extensions.OfType<X509EnhancedKeyUsageExtension>()) {
                hasEkuExtension = true;
                foreach (var oid in extension.EnhancedKeyUsages) {
                    if (oid.Value.Equals(ServerAuthenticationOid, StringComparison.Ordinal)) {
                        return true;
                    }
                }
            }

            return !hasEkuExtension;
        }

        internal static bool DoesCertificateHaveAnAccessiblePrivateKey(X509Certificate2 certificate)
            => certificate.HasPrivateKey;

        private static void DisposeCertificates(X509Certificate2Collection certificates, X509Certificate2 except) {
            if (certificates != null) {
                foreach (var certificate in certificates) {
                    if (!certificate.Equals(except)) {
                        certificate.Dispose();
                    }
                }
            }
        }
    }
}
