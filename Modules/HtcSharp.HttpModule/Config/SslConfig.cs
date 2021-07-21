using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace HtcSharp.HttpModule.Config {
    public class SslConfig {
        public string Certificate { get; set; }

        public string Key { get; set; }

        public string Password { get; set; }

        public static SslConfig ParseConfig(JsonElement jsonElement) {
            var sslConfig = new SslConfig();
            foreach (var property in jsonElement.EnumerateObject()) {
                string propertyName = property.Name.ToLower();
                switch (propertyName) {
                    case "certificate": {
                        string cert =  property.Value.GetString();
                        if (string.IsNullOrEmpty(cert)) continue;
                        sslConfig.Certificate = Path.IsPathFullyQualified(cert) ? File.ReadAllText(cert, Encoding.UTF8) : cert;
                        break;
                    }
                    case "key": {
                        string key =  property.Value.GetString();
                        if (string.IsNullOrEmpty(key)) continue;
                        sslConfig.Key = Path.IsPathFullyQualified(key) ? File.ReadAllText(key, Encoding.UTF8) : key;
                        break;
                    }
                    case "password":
                        sslConfig.Password = property.Value.GetString();
                        break;
                }
            }

            return sslConfig;
        }

        public X509Certificate2 GetCertificate() {
            return string.IsNullOrEmpty(Password) ? X509Certificate2.CreateFromPem(Certificate, Key) : X509Certificate2.CreateFromEncryptedPem(Certificate, Key, Password);
        }
    }
}