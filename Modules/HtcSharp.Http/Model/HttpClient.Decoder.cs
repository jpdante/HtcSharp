using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtcSharp.Http.Model {
    public partial class HttpClient {
        private class Decoder {
            private readonly HttpClient _owner;
            private readonly StreamReader _streamReader;

            public Decoder(HttpClient owner) {
                _owner = owner;
                _streamReader = new StreamReader(_owner._stream);
            }

            public async Task RunAsync() {
                var stringBuilder = new StringBuilder();
                int current, previous = -1, lineIndex = 0;
                while ((current = _streamReader.Read()) != -1) {
                    stringBuilder.Append((char)current);
                    if (previous == '\r' && current == '\n') {
                        if (stringBuilder.Length == 0) break;
                        if (lineIndex == 0) {
                            var httpVersionArgs = Regex.Split(stringBuilder.ToString(), "[ \t\n\x0b\r\f]");
                            foreach (var i in httpVersionArgs) {
                               Logger.Error(i); 
                            }
                            if (httpVersionArgs[2].IndexOf("HTTP/", StringComparison.Ordinal) == 0 && httpVersionArgs[2].IndexOf('.') > 5) {
                                var versionData = httpVersionArgs[2].Substring(5).Split('.');
                                var versionInt = new int[2];
                                if (int.TryParse(versionData[0], out versionInt[0]) && int.TryParse(versionData[1], out versionInt[1])) {

                                }
                            }
                        } else DecodeProtocolHTTP1(stringBuilder.ToString().Remove(stringBuilder.Length - 2, 2), lineIndex);
                        stringBuilder.Clear();
                        lineIndex++;
                    }
                    previous = current;
                }
            }

            private void DecodeProtocolHTTP1(string data, int lineIndex) {
                Logger.Info(data.Replace("\n", "\\n").Replace("\r", "\\r"));
            }

            private void DecodeProtocolHTTP2(string data, int lineIndex) {
                Logger.Info(data.Replace("\n", "\\n").Replace("\r", "\\r"));
            }
        }
    }
}
