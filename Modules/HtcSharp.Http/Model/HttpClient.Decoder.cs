using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtcSharp.Http.Model.Http;
using HtcSharp.Http.Model.Http.Enum;

namespace HtcSharp.Http.Model {
    public partial class HttpClient {
        private class Decoder {
            private readonly HttpClient _owner;
            private readonly StreamReader _streamReader;

            public Decoder(HttpClient owner) {
                _owner = owner;
                _streamReader = new StreamReader(_owner._stream);
            }

            public async ValueTask<HttpRequest> DecodeRequest() {
                return new HttpRequest();
            }

            public async void RunAsync() {
                var stringBuilder = new StringBuilder();
                int current, previous = -1, lineIndex = 0;
                while ((current = _streamReader.Read()) != -1) {
                    stringBuilder.Append((char)current);
                    if (previous == '\r' && current == '\n') {
                        if (lineIndex == 0) {
                            var httpVersionArgs = Regex.Split(stringBuilder.ToString(), "[ \t\n\x0b\r\f]");
                            _owner._httpRequest = new HttpRequest {
                                Protocol = httpVersionArgs[2]
                            };
                            switch (httpVersionArgs[2]) {
                                case "HTTP/1.0":
                                    _owner._httpRequest.HttpVersion = HttpVersion.HTTP_1_0;
                                    break;
                                case "HTTP/1.1":
                                    _owner._httpRequest.HttpVersion = HttpVersion.HTTP_1_1;
                                    break;
                                case "HTTP/2.0":
                                    _owner._httpRequest.HttpVersion = HttpVersion.HTTP_2_0;
                                    break;
                                default:
                                    throw new Exception("Unknown protocol version / type");
                            }
                            switch (httpVersionArgs[0]) {
                                case "GET":
                                    _owner._httpRequest.Method = RequestMethod.GET;
                                    break;
                                case "HEAD":
                                    _owner._httpRequest.Method = RequestMethod.HEAD;
                                    break;
                                case "POST":
                                    _owner._httpRequest.Method = RequestMethod.POST;
                                    break;
                                case "OPTIONS":
                                    _owner._httpRequest.Method = RequestMethod.OPTIONS;
                                    break;
                                case "DELETE":
                                    _owner._httpRequest.Method = RequestMethod.DELETE;
                                    break;
                                case "TRACE":
                                    _owner._httpRequest.Method = RequestMethod.TRACE;
                                    break;
                                case "CONNECT":
                                    _owner._httpRequest.Method = RequestMethod.CONNECT;
                                    break;
                                default:
                                    throw new Exception("Unknown request type");
                            }
                            /*if (httpVersionArgs[2].IndexOf("HTTP/", StringComparison.Ordinal) == 0 && httpVersionArgs[2].IndexOf('.') > 5) {
                                var versionData = httpVersionArgs[2].Substring(5).Split('.');
                                var versionInt = new int[2];
                                if (int.TryParse(versionData[0], out versionInt[0]) && int.TryParse(versionData[1], out versionInt[1])) {
                                    Logger.Info($"Http version: HTTP {versionInt[0]}.{versionInt[1]}");
                                }

                            }*/
                        } else {
                            if (stringBuilder.ToString() == "\r\n") break;
                            switch (_owner._httpRequest.HttpVersion) {
                                case HttpVersion.HTTP_1_0:
                                    DecodeProtocolHttp1_0(stringBuilder.ToString().Substring(0, stringBuilder.Length - 2));
                                    break;
                                case HttpVersion.HTTP_1_1:
                                    DecodeProtocolHttp1_1(stringBuilder.ToString().Substring(0, stringBuilder.Length - 2));
                                    break;
                                case HttpVersion.HTTP_2_0:
                                    DecodeProtocolHttp2_0(stringBuilder.ToString().Substring(0, stringBuilder.Length - 2));
                                    break;
                                default:
                                    throw new Exception("Unknown protocol version / type");
                            }
                        }
                        stringBuilder.Clear();
                        lineIndex++;
                    }
                    previous = current;
                }
                // Stream Body
                _owner.Dispose();
            }

            private void DecodeProtocolHttp1_0(string data) {
                var dataSplit = data.Split(": ", 2);
                _owner._httpRequest.Headers.Add(dataSplit[0], dataSplit[1]);
                Logger.Debug($"{dataSplit[0]}: {dataSplit[1]}");
            }

            private void DecodeProtocolHttp1_1(string data) {
                var dataSplit = data.Split(": ", 2);
                _owner._httpRequest.Headers.Add(dataSplit[0], dataSplit[1]);
                Logger.Debug($"{dataSplit[0]}: {dataSplit[1]}");
            }

            private void DecodeProtocolHttp2_0(string data) {
                var dataSplit = data.Split(": ", 2);
                _owner._httpRequest.Headers.Add(dataSplit[0], dataSplit[1]);
                Logger.Debug($"{dataSplit[0]}: {dataSplit[1]}");
            }
        }
    }
}
