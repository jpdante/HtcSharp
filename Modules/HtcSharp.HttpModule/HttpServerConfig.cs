using System.Collections.Generic;
using HtcSharp.HttpModule.Routing;
using HtcSharp.HttpModule.Routing.Error;

namespace HtcSharp.HttpModule {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    internal class HttpServerConfig {
        public List<string> Endpoints { get; }
        public List<string> Domains { get; }
        public string RootPath { get; }
        public bool UseSsl { get; }
        public string Certificate { get; }
        public string CertificatePassword { get; }
        public HttpLocationManager LocationManager { get; }
        public ErrorMessageManager ErrorMessageManager { get; }
        public HttpServerInfo HttpServerInfo { get; }

        public HttpServerConfig(List<string> endPoints, List<string> domains, string rootPath, bool useSSL, string certificate, string certificatePassword, HttpLocationManager locationManager, ErrorMessageManager errorMessageManager) {
            Endpoints = endPoints;
            Domains = domains;
            RootPath = rootPath;
            UseSsl = useSSL;
            Certificate = certificate;
            CertificatePassword = certificatePassword;
            LocationManager = locationManager;
            ErrorMessageManager = errorMessageManager;
            HttpServerInfo = new HttpServerInfo(Endpoints.AsReadOnly(), Domains.AsReadOnly(), RootPath, UseSsl, LocationManager, ErrorMessageManager);
        }
    }

    public class HttpServerInfo {
        public IReadOnlyCollection<string> Endpoints { get; }
        public IReadOnlyCollection<string> Domains { get; }
        public string RootPath { get; }
        public bool UseSsl { get; }
        public HttpLocationManager LocationManager { get; }
        public ErrorMessageManager ErrorMessageManager { get; }

        public HttpServerInfo(IReadOnlyCollection<string> endPoints, IReadOnlyCollection<string> domains, string rootPath, bool useSSL, HttpLocationManager locationManager, ErrorMessageManager errorMessageManager) {
            Endpoints = endPoints;
            Domains = domains;
            RootPath = rootPath;
            UseSsl = useSSL;
            LocationManager = locationManager;
            ErrorMessageManager = errorMessageManager;
        }
    }
}