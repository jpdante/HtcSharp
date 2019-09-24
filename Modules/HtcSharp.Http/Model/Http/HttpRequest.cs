using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtcSharp.Http.Model.Http.Enum;

namespace HtcSharp.Http.Model.Http {
    public class HttpRequest {
        public string Path { get; internal set; }
        public Stream Body { get; internal set; }
        public string Host { get; internal set; }
        public string Query { get; internal set; }
        public bool IsHttps { get; internal set; }
        public string Scheme { get; internal set; }
        public RequestMethod Method { get; internal set; }
        public string Protocol { get; internal set; }
        public string QueryString { get; internal set; }
        public HttpVersion HttpVersion { get; internal set; }
        public string ContentType { get; internal set; }
        public long ContentLength { get; internal set; }
        public Dictionary<string, string> Cookies { get; internal set; }
        public Dictionary<string, string> Headers { get; internal set; }

        public HttpRequest() {
            Headers = new Dictionary<string, string>();
            Cookies = new Dictionary<string, string>();
        }
    }
}
