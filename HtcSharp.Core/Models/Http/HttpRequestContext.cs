using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Models.Http.Utils;
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

namespace HtcSharp.Core.Models.Http {
    public class HttpRequestContext {

        private readonly HttpRequest _request;

        public Stream InputStream => _request.Body;
        public long? ContentLength => _request.ContentLength;
        public ContentType ContentType => ContentType.DEFAULT.FromString(_request.ContentType);
        public string Host => _request.Host.ToString();
        public bool IsHttps => _request.IsHttps;
        public HttpMethod Method => HttpMethod.GET.FromString(_request.Method);
        public string Path => _request.Path.ToString();
        public string PathBase => _request.PathBase.ToString();
        public string Protocol => _request.Protocol;
        public string QueryString => _request.QueryString.ToString();
        public string Scheme => _request.Scheme;
        public List<HtcFile> Files;
        public Dictionary<string, string> Post;
        public Dictionary<string, string> Cookies;
        public Dictionary<string, string> Query;
        public Dictionary<string, string> Headers;

        public HttpRequestContext(HttpRequest request) {
            _request = request;
            Files = new List<HtcFile>();
            Post = new Dictionary<string, string>();
            Cookies = new Dictionary<string, string>();
            Query = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            foreach (var key in _request.Cookies.Keys) {
                Cookies.Add(key, _request.Cookies[key]);
            }
            foreach (var key in _request.Query.Keys) {
                Query.Add(key, _request.Query[key]);
            }
            foreach (var key in _request.Headers.Keys) {
                Headers.Add(key, _request.Headers[key]);
            }
            if(Method == HttpMethod.POST) {
                if(ContentType == ContentType.FormUrlEncoded || ContentType == ContentType.MultipartFormData) {
                    foreach (var key in _request.Form.Keys) {
                        Post.Add(key, _request.Form[key]);
                    }
                    foreach (var file in _request.Form.Files) {
                        Files.Add(new HtcFile(file));
                    }
                } else if (ContentType == ContentType.JSON) {
                    using (var reader = new StreamReader(InputStream, Encoding.UTF8)) {
                        var json = reader.ReadToEnd();
                        try {
                            var post = JObject.Parse(json);
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
                        var array = (JArray)obj;
                        DoJsonAdd($"{key}.length", array.Count);
                        for (var i = 0; i < array.Count; i++) {
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
                        var obj2 = obj.ToObject<JObject>();
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } catch {
                // ignored
            }
        }
    }
}
