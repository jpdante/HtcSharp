using HTCSharp.Core.Helpers.Http;
using HTCSharp.Core.Models.Http.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
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
                if(ContentType == ContentType.FormUrlEncoded || ContentType == ContentType.MultipartFormData) {
                    foreach (string key in Request.Form.Keys) {
                        Post.Add(key, Request.Form[key]);
                    }
                    foreach (var file in Request.Form.Files) {
                        Files.Add(new HTCFile(file));
                    }
                } else if (ContentType == ContentType.JSON) {
                    using (StreamReader reader = new StreamReader(InputStream, Encoding.UTF8)) {
                        string json = reader.ReadToEnd();
                        try {
                            JObject post = JObject.Parse(json);
                            foreach (var item in post) {
                                DoJsonAdd(item.Key, item.Value);
                            }
                        } catch { }
                    }
                }
            }
        }

        public void DoJsonAdd(string key, JToken obj) {
            try {
                switch (obj.Type) {
                    case JTokenType.Array:
                        JArray array = (JArray)obj;
                        DoJsonAdd($"{key}.length", array.Count);
                        for (int i = 0; i < array.Count; i++) {
                            DoJsonAdd($"{key}.{i}", array[i]);
                        }
                        break;
                    case JTokenType.Boolean:
                        Post.Add(key, obj.ToObject<bool>().ToString());
                        break;
                    case JTokenType.Bytes:
                        Post.Add(key, obj.ToObject<object>().ToString());
                        break;
                    case JTokenType.Comment:
                        Post.Add(key, obj.ToObject<object>().ToString());
                        break;
                    case JTokenType.Constructor:
                        Post.Add(key, obj.ToObject<object>().ToString());
                        break;
                    case JTokenType.Date:
                        Post.Add(key, obj.ToObject<DateTime>().ToString());
                        break;
                    case JTokenType.Float:
                        Post.Add(key, obj.ToObject<float>().ToString());
                        break;
                    case JTokenType.Guid:
                        Post.Add(key, obj.ToObject<Guid>().ToString());
                        break;
                    case JTokenType.Integer:
                        Post.Add(key, obj.ToObject<int>().ToString());
                        break;
                    case JTokenType.None:
                        Post.Add(key, "");
                        break;
                    case JTokenType.Null:
                        break;
                    case JTokenType.Object:
                        JObject obj2 = obj.ToObject<JObject>();
                        foreach (var item in obj2) {
                            DoJsonAdd($"{key}.{item.Key}", item.Value);
                        }
                        Post.Add(key, obj.ToObject<JObject>().ToString());
                        break;
                    case JTokenType.Property:
                        Post.Add(key, obj.ToObject<object>().ToString());
                        break;
                    case JTokenType.Raw:
                        Post.Add(key, obj.ToObject<object>().ToString());
                        break;
                    case JTokenType.String:
                        Post.Add(key, obj.ToObject<string>());
                        break;
                    case JTokenType.TimeSpan:
                        Post.Add(key, obj.ToObject<TimeSpan>().ToString());
                        break;
                    case JTokenType.Undefined:
                        Post.Add(key, obj.ToObject<object>().ToString());
                        break;
                    case JTokenType.Uri:
                        Post.Add(key, obj.ToObject<Uri>().ToString());
                        break;
                }
            } catch { }
        }
    }
}
