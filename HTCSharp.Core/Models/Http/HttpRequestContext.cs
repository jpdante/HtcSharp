using HTCSharp.Core.Helpers.Http;
using HTCSharp.Core.Models.Http.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace HTCSharp.Core.Models.Http {
    public class HttpRequestContext {

        private HttpRequest Request;
        private Dictionary<string, string> PostData;

        public Stream InputStream { get { return Request.Body; } }
        public long? ContentLength { get { return Request.ContentLength; } }
        public ContentType ContentType { get { return ContentType.DEFAULT.FromString(Request.ContentType); } }
        public string Host { get { return Request.Host.ToString(); } }
        public bool IsHttps { get { return Request.IsHttps; } }
        public HttpMethod Method { get { return HttpMethod.GET.FromString(Request.Method); } }
        public string Path { get { return Request.Path.ToString(); } }
        public string PathBase { get { return Request.PathBase.ToString(); } }
        public string Protocol { get { return Request.Protocol; } }
        public string QueryString { get { return Request.QueryString.ToString(); } }
        public string Scheme { get { return Request.Scheme; } }
        public List<HTCFile> Files;
        public Dictionary<string, string> Post;
        public Dictionary<string, string> Cookies;
        public Dictionary<string, string> Query;
        public Dictionary<string, string> Headers;

        public HttpRequestContext(HttpRequest request) {
            Request = request;
            Files = new List<HTCFile>();
            Post = new Dictionary<string, string>();
            Cookies = new Dictionary<string, string>();
            Query = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            foreach (string key in Request.Cookies.Keys) {
                Cookies.Add(key, Request.Cookies[key]);
            }
            foreach (string key in Request.Query.Keys) {
                Query.Add(key, Request.Query[key]);
            }
            foreach (string key in Request.Headers.Keys) {
                Headers.Add(key, Request.Headers[key]);
            }
            if(Method == HttpMethod.POST) {
                foreach (string key in Request.Form.Keys) {
                    Post.Add(key, Request.Form[key]);
                }
                foreach (var file in Request.Form.Files) {
                    Files.Add(new HTCFile(file));
                }
            }
        }
    }
}
