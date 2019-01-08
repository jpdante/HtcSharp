using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCResponseHeaders {
        private IHeaderDictionary Header;

        public HTCResponseHeaders(IHeaderDictionary header) {
            Header = header;
        }

        public string this[string key] {
            get {
                return Header[key];
            }
            set {
                Header.Add(key, value);
            }
        }

        public void Add(string key, string value) {
            Header.Add(key, value);
        }

        public void Add(string key, StringValues value) {
            Header.Add(key, value);
        }

        public void Clear() {
            Header.Clear();
        }

        public bool ContainsKey(string key) {
            return Header.ContainsKey(key);
        }

        public long? ContentLength { get { return Header.ContentLength; } }
        public int Count { get { return Header.Count; } }
        public ICollection<string> Keys { get { return Header.Keys; } }
        public ICollection<StringValues> Values { get { return Header.Values; } }
    }
}
