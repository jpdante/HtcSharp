using System;
using System.Globalization;

namespace HtcSharp.HttpModule2.Connection.Address {
    public class BindingAddress {
        private const string UnixPipeHostPrefix = "unix:/";

        public string Host { get; private set; }
        public string PathBase { get; private set; }
        public int Port { get; internal set; }
        public string Scheme { get; private set; }

        public bool IsUnixPipe => Host.StartsWith(UnixPipeHostPrefix, StringComparison.Ordinal);

        public string UnixPipePath {
            get {
                if (!IsUnixPipe) throw new InvalidOperationException("Binding address is not a unix pipe.");

                return Host.Substring(UnixPipeHostPrefix.Length - 1);
            }
        }

        public override string ToString() {
            if (IsUnixPipe) return Scheme.ToLowerInvariant() + "://" + Host.ToLowerInvariant();
            return Scheme.ToLowerInvariant() + "://" + Host.ToLowerInvariant() + ":" +
                   Port.ToString(CultureInfo.InvariantCulture) + PathBase;
        }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override bool Equals(object obj) {
            if (!(obj is BindingAddress other)) return false;
            return string.Equals(Scheme, other.Scheme, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase)
                   && Port == other.Port
                   && PathBase == other.PathBase;
        }

        public static BindingAddress Parse(string address) {
            address ??= string.Empty;

            var schemeDelimiterStart = address.IndexOf("://", StringComparison.Ordinal);
            if (schemeDelimiterStart < 0) throw new FormatException($"Invalid url: '{address}'");
            var schemeDelimiterEnd = schemeDelimiterStart + "://".Length;

            var isUnixPipe = address.IndexOf(UnixPipeHostPrefix, schemeDelimiterEnd, StringComparison.Ordinal) ==
                             schemeDelimiterEnd;

            int pathDelimiterStart;
            int pathDelimiterEnd;
            if (!isUnixPipe) {
                pathDelimiterStart = address.IndexOf("/", schemeDelimiterEnd, StringComparison.Ordinal);
                pathDelimiterEnd = pathDelimiterStart;
            } else {
                pathDelimiterStart = address.IndexOf(":", schemeDelimiterEnd + UnixPipeHostPrefix.Length,
                    StringComparison.Ordinal);
                pathDelimiterEnd = pathDelimiterStart + ":".Length;
            }

            if (pathDelimiterStart < 0) pathDelimiterStart = pathDelimiterEnd = address.Length;

            var serverAddress = new BindingAddress {Scheme = address.Substring(0, schemeDelimiterStart)};

            var hasSpecifiedPort = false;
            if (!isUnixPipe) {
                var portDelimiterStart = address.LastIndexOf(":", pathDelimiterStart - 1,
                    pathDelimiterStart - schemeDelimiterEnd, StringComparison.Ordinal);
                if (portDelimiterStart >= 0) {
                    var portDelimiterEnd = portDelimiterStart + ":".Length;

                    var portString = address.Substring(portDelimiterEnd, pathDelimiterStart - portDelimiterEnd);
                    if (int.TryParse(portString, NumberStyles.Integer, CultureInfo.InvariantCulture, out var portNumber)) {
                        hasSpecifiedPort = true;
                        serverAddress.Host =
                            address.Substring(schemeDelimiterEnd, portDelimiterStart - schemeDelimiterEnd);
                        serverAddress.Port = portNumber;
                    }
                }

                if (!hasSpecifiedPort) {
                    if (string.Equals(serverAddress.Scheme, "http", StringComparison.OrdinalIgnoreCase))
                        serverAddress.Port = 80;
                    else if (string.Equals(serverAddress.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                        serverAddress.Port = 443;
                }
            }

            if (!hasSpecifiedPort)
                serverAddress.Host = address.Substring(schemeDelimiterEnd, pathDelimiterStart - schemeDelimiterEnd);

            if (string.IsNullOrEmpty(serverAddress.Host)) throw new FormatException($"Invalid url: '{address}'");

            serverAddress.PathBase = address[^1] == '/' ? address.Substring(pathDelimiterEnd, address.Length - pathDelimiterEnd - 1) : address.Substring(pathDelimiterEnd);

            return serverAddress;
        }
    }
}