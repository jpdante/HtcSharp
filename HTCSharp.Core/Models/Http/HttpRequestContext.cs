using HTCSharp.Core.Models.Http.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace HTCSharp.Core.Models.Http {
    public class HttpRequestContext {

        private HttpRequest Request;
        private Dictionary<string, string> PostData;

        public Stream InputStream { get { return Request.Body; } }
        public long? ContentLength { get { return Request.ContentLength; } }
        public string ContentType { get { return Request.ContentType; } }
        public string Host { get { return Request.Host.ToString(); } }
        public bool IsHttps { get { return Request.IsHttps; } }
        public string Method { get { return Request.Method; } }
        public string Path { get { return Request.Path.ToString(); } }
        public string PathBase { get { return Request.PathBase.ToString(); } }
        public string Protocol { get { return Request.Protocol; } }
        public string QueryString { get { return Request.QueryString.ToString(); } }
        public string Scheme { get { return Request.Scheme; } }
        public HTCRequestHeaders Headers { get; }
        public HTCRequestQuery Query { get; }
        public HTCRequestCookies Cookies { get; }
        public Dictionary<string, string> Post {
            get {
                if (Request.Method.Equals("Post", StringComparison.CurrentCultureIgnoreCase)) {
                    if(PostData == null) {
                        PostData = new Dictionary<string, string>();
                        using (StreamReader reader = new StreamReader(InputStream)) {
                            foreach (var param in reader.ReadToEnd().Split("&")) {
                                string[] kvPair = param.Split('=');
                                if (kvPair.Length < 2) continue;
                                PostData.Add(kvPair[0], HttpUtility.UrlDecode(kvPair[1]));
                            }
                        }
                    }
                    return PostData;
                } else {
                    return new Dictionary<string, string>(); ;
                }
            }
        }

        public HttpRequestContext(HttpRequest request) {
            Request = request;
            Headers = new HTCRequestHeaders(Request.Headers);
            Cookies = new HTCRequestCookies(Request.Cookies);
            Query = new HTCRequestQuery(Request.Query);
        }
    }
}
