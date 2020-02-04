using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Http.Http.Abstractions;
using HtcSharp.HttpModule.Infrastructure.Certificate;
using HtcSharp.HttpModule.Infrastructure.Extensions;
using HtcSharp.HttpModule.Infrastructure.Features;
using HtcSharp.HttpModule.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HtcSharp.HttpModule.Infrastructure {
    internal class HttpsConnectionMiddleware {
        private readonly ConnectionDelegate _next;
        private readonly HttpsConnectionAdapterOptions _options;
        private readonly ILogger _logger;
        private readonly X509Certificate2 _serverCertificate;
        private readonly Func<ConnectionContext, string, X509Certificate2> _serverCertificateSelector;

        public HttpsConnectionMiddleware(ConnectionDelegate next, HttpsConnectionAdapterOptions options)
          : this(next, options, loggerFactory: NullLoggerFactory.Instance) {
        }

        public HttpsConnectionMiddleware(ConnectionDelegate next, HttpsConnectionAdapterOptions options, ILoggerFactory loggerFactory) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            // This configuration will always fail per-request, preemptively fail it here. See HttpConnection.SelectProtocol().
            if (options.HttpProtocols == HttpProtocols.Http2) {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    throw new NotSupportedException("HTTP/2 over TLS is not supported on macOS due to missing ALPN support.");
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version < new Version(6, 2)) {
                    throw new NotSupportedException("HTTP/2 over TLS is not supported on Windows 7 due to missing ALPN support.");
                }
            }

            _next = next;
            // capture the certificate now so it can't be switched after validation
            _serverCertificate = options.ServerCertificate;
            _serverCertificateSelector = options.ServerCertificateSelector;
            if (_serverCertificate == null && _serverCertificateSelector == null) {
                throw new ArgumentException("The server certificate parameter is required.", nameof(options));
            }

            // If a selector is provided then ignore the cert, it may be a default cert.
            if (_serverCertificateSelector != null) {
                // SslStream doesn't allow both.
                _serverCertificate = null;
            } else {
                EnsureCertificateIsAllowedForServerAuth(_serverCertificate);
            }

            _options = options;
            _logger = loggerFactory?.CreateLogger<HttpsConnectionMiddleware>();
        }
        public Task OnConnectionAsync(ConnectionContext context) {
            return Task.Run(() => InnerOnConnectionAsync(context));
        }

        private async Task InnerOnConnectionAsync(ConnectionContext context) {
            bool certificateRequired;
            var feature = new TlsConnectionFeature();
            context.Features.Set<ITlsConnectionFeature>(feature);
            context.Features.Set<ITlsHandshakeFeature>(feature);

            var memoryPool = context.Features.Get<IMemoryPoolFeature>()?.MemoryPool;

            var inputPipeOptions = new StreamPipeReaderOptions
            (
                pool: memoryPool,
                bufferSize: memoryPool.GetMinimumSegmentSize(),
                minimumReadSize: memoryPool.GetMinimumAllocSize(),
                leaveOpen: true
            );

            var outputPipeOptions = new StreamPipeWriterOptions
            (
                pool: memoryPool,
                leaveOpen: true
            );

            SslDuplexPipe sslDuplexPipe = null;

            if (_options.ClientCertificateMode == ClientCertificateMode.NoCertificate) {
                sslDuplexPipe = new SslDuplexPipe(context.Transport, inputPipeOptions, outputPipeOptions);
                certificateRequired = false;
            } else {
                sslDuplexPipe = new SslDuplexPipe(context.Transport, inputPipeOptions, outputPipeOptions, s => new SslStream(s,
                    leaveInnerStreamOpen: false,
                    userCertificateValidationCallback: (sender, certificate, chain, sslPolicyErrors) => {
                        if (certificate == null) {
                            return _options.ClientCertificateMode != ClientCertificateMode.RequireCertificate;
                        }

                        if (_options.ClientCertificateValidation == null) {
                            if (sslPolicyErrors != SslPolicyErrors.None) {
                                return false;
                            }
                        }

                        var certificate2 = ConvertToX509Certificate2(certificate);
                        if (certificate2 == null) {
                            return false;
                        }

                        if (_options.ClientCertificateValidation != null) {
                            if (!_options.ClientCertificateValidation(certificate2, chain, sslPolicyErrors)) {
                                return false;
                            }
                        }

                        return true;
                    }));

                certificateRequired = true;
            }

            var sslStream = sslDuplexPipe.Stream;

            using (var cancellationTokeSource = new CancellationTokenSource(_options.HandshakeTimeout))
            using (cancellationTokeSource.Token.UnsafeRegister(state => ((ConnectionContext)state).Abort(), context)) {
                try {
                    // Adapt to the SslStream signature
                    ServerCertificateSelectionCallback selector = null;
                    if (_serverCertificateSelector != null) {
                        selector = (sender, name) => {
                            context.Features.Set(sslStream);
                            var cert = _serverCertificateSelector(context, name);
                            if (cert != null) {
                                EnsureCertificateIsAllowedForServerAuth(cert);
                            }
                            return cert;
                        };
                    }

                    var sslOptions = new SslServerAuthenticationOptions {
                        ServerCertificate = _serverCertificate,
                        ServerCertificateSelectionCallback = selector,
                        ClientCertificateRequired = certificateRequired,
                        EnabledSslProtocols = _options.SslProtocols,
                        CertificateRevocationCheckMode = _options.CheckCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                        ApplicationProtocols = new List<SslApplicationProtocol>()
                    };

                    // This is order sensitive
                    if ((_options.HttpProtocols & HttpProtocols.Http2) != 0) {
                        sslOptions.ApplicationProtocols.Add(SslApplicationProtocol.Http2);
                        // https://tools.ietf.org/html/rfc7540#section-9.2.1
                        sslOptions.AllowRenegotiation = false;
                    }

                    if ((_options.HttpProtocols & HttpProtocols.Http1) != 0) {
                        sslOptions.ApplicationProtocols.Add(SslApplicationProtocol.Http11);
                    }

                    _options.OnAuthenticate?.Invoke(context, sslOptions);

                    await sslStream.AuthenticateAsServerAsync(sslOptions, CancellationToken.None);
                } catch (OperationCanceledException) {
                    _logger?.LogDebug(2, "Authentication of the HTTPS connection timed out.");
                    await sslStream.DisposeAsync();
                    return;
                } catch (IOException ex) {
                    _logger?.LogDebug(1, ex, "Failed to authenticate HTTPS connection.");
                    await sslStream.DisposeAsync();
                    return;
                } catch (AuthenticationException ex) {
                    if (_serverCertificate == null ||
                        !CertificateManager.IsHttpsDevelopmentCertificate(_serverCertificate) ||
                        CertificateManager.CheckDeveloperCertificateKey(_serverCertificate)) {
                        _logger?.LogDebug(1, ex, "Failed to authenticate HTTPS connection.");
                    } else {
                        _logger?.LogError(3, ex, "The ASP.NET Core developer certificate is in an invalid state. To fix this issue, run the following commands 'dotnet dev-certs https --clean' and 'dotnet dev-certs https' to remove all existing ASP.NET Core development certificates and create a new untrusted developer certificate. On macOS or Windows, use 'dotnet dev-certs https --trust' to trust the new certificate.");
                    }

                    await sslStream.DisposeAsync();
                    return;
                }
            }

            feature.ApplicationProtocol = sslStream.NegotiatedApplicationProtocol.Protocol;
            context.Features.Set<ITlsApplicationProtocolFeature>(feature);
            feature.ClientCertificate = ConvertToX509Certificate2(sslStream.RemoteCertificate);
            feature.CipherAlgorithm = sslStream.CipherAlgorithm;
            feature.CipherStrength = sslStream.CipherStrength;
            feature.HashAlgorithm = sslStream.HashAlgorithm;
            feature.HashStrength = sslStream.HashStrength;
            feature.KeyExchangeAlgorithm = sslStream.KeyExchangeAlgorithm;
            feature.KeyExchangeStrength = sslStream.KeyExchangeStrength;
            feature.Protocol = sslStream.SslProtocol;

            var originalTransport = context.Transport;

            try {
                context.Transport = sslDuplexPipe;

                // Disposing the stream will dispose the sslDuplexPipe
                await using (sslStream)
                await using (sslDuplexPipe) {
                    await _next(context);
                    // Dispose the inner stream (SslDuplexPipe) before disposing the SslStream
                    // as the duplex pipe can hit an ODE as it still may be writing.
                }
            } finally {
                // Restore the original so that it gets closed appropriately
                context.Transport = originalTransport;
            }
        }

        private static void EnsureCertificateIsAllowedForServerAuth(X509Certificate2 certificate) {
            if (!CertificateLoader.IsCertificateAllowedForServerAuth(certificate)) {
                throw new InvalidOperationException($"Certificate {certificate.Thumbprint} cannot be used as an SSL server certificate. It has an Extended Key Usage extension but the usages do not include Server Authentication (OID 1.3.6.1.5.5.7.3.1).");
            }
        }

        private static X509Certificate2 ConvertToX509Certificate2(X509Certificate certificate) {
            if (certificate == null) {
                return null;
            }

            if (certificate is X509Certificate2 cert2) {
                return cert2;
            }

            return new X509Certificate2(certificate);
        }

        private class SslDuplexPipe : DuplexPipeStreamAdapter<SslStream> {
            public SslDuplexPipe(IDuplexPipe transport, StreamPipeReaderOptions readerOptions, StreamPipeWriterOptions writerOptions)
                : this(transport, readerOptions, writerOptions, s => new SslStream(s)) {
            }

            public SslDuplexPipe(IDuplexPipe transport, StreamPipeReaderOptions readerOptions, StreamPipeWriterOptions writerOptions, Func<System.IO.Stream, SslStream> factory) :
                base(transport, readerOptions, writerOptions, factory) {
            }
        }
    }
}
