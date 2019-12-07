using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using HtcSharp.HttpModule2.Connection;

namespace HtcSharp.HttpModule2.Core {
    public class HttpsConnectionAdapterOptions {
        private TimeSpan _handshakeTimeout;

        public HttpsConnectionAdapterOptions() {
            ClientCertificateMode = ClientCertificateMode.NO_CERTIFICATE;
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11;
            HandshakeTimeout = TimeSpan.FromSeconds(10);
        }

        public X509Certificate2 ServerCertificate { get; set; }

        public Func<ConnectionContext, string, X509Certificate2> ServerCertificateSelector { get; set; }

        public ClientCertificateMode ClientCertificateMode { get; set; }

        public Func<X509Certificate2, X509Chain, SslPolicyErrors, bool> ClientCertificateValidation { get; set; }

        public SslProtocols SslProtocols { get; set; }

        internal HttpProtocols HttpProtocols { get; set; }

        public bool CheckCertificateRevocation { get; set; }

        public void AllowAnyClientCertificate() {
            ClientCertificateValidation = (_, __, ___) => true;
        }

        public Action<ConnectionContext, SslServerAuthenticationOptions> OnAuthenticate { get; set; }

        public TimeSpan HandshakeTimeout {
            get => _handshakeTimeout;
            set {
                if (value <= TimeSpan.Zero && value != Timeout.InfiniteTimeSpan) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be a positive TimeSpan.");
                }
                _handshakeTimeout = value != Timeout.InfiniteTimeSpan ? value : TimeSpan.MaxValue;
            }
        }
    }
}
