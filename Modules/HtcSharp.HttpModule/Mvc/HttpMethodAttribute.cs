using System;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpMethodAttribute : Attribute {

        public string Method { get; }

        public string Path { get; }

        public HttpMethodAttribute(string method, string path) {
            Method = method;
            Path = path;
        }
    }
}
