using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.Http.Model.Http;

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
                /*var _buffer = new byte[9024];
                var bytesRead = await _owner._stream.ReadAsync(_buffer, 0, _buffer.Length);
                var sBuffer = Encoding.ASCII.GetString(_buffer, 0, bytesRead);
                Logger.Fatal(sBuffer);
                return;*/
                try {
                    _streamReader.BaseStream.ReadTimeout = 2000;
                    var dataSplit = (await _streamReader.ReadLineAsync()).Split(' ');
                    _owner._httpRequest = new HttpRequest {
                        Method = dataSplit[0],
                        QueryString = dataSplit[1],
                        HttpVersion = dataSplit[2]
                    };
                    while (!_streamReader.EndOfStream) {
                        var line = await _streamReader.ReadLineAsync();
                        dataSplit = line.Split(": ");
                        if (dataSplit.Length == 2) {
                            _owner._httpRequest.Headers.Add(dataSplit[0], dataSplit[1]);
                            Logger.Info($"{dataSplit[0]}: {dataSplit[1]}");
                            //_owner._stream.Write(Encoding.ASCII.GetBytes("GET 100 HTTP/1.1"));
                            if (dataSplit[0].Equals("Host", StringComparison.CurrentCultureIgnoreCase)) {
                                _owner._httpRequest.Host = dataSplit[1];
                            } else if (dataSplit[0].Equals("Connection", StringComparison.CurrentCultureIgnoreCase)) {

                            } else {

                            }
                        } else {
                            //SUICIDIO
                        }
                    }
                    ObjectDump.DumpToLogger(_owner._httpRequest);
                } catch(Exception ex) {
                    _owner.Dispose();
                }
            }
        }
    }
}
