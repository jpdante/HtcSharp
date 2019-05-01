using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Models.Http.Utils {
    public class HtcResponseHeaders {
        private readonly IHeaderDictionary _header;

        public HtcResponseHeaders(IHeaderDictionary header) {
            _header = header;
        }

        public string this[string key] {
            get => _header[key];
            set => _header.Add(key, value);
        }

        public void Add(string key, string value) {
            _header.Add(key, value);
        }

        public void Clear() {
            _header.Clear();
        }

        public bool ContainsKey(string key) {
            return _header.ContainsKey(key);
        }

        public long? ContentLength => _header.ContentLength;
        public int Count => _header.Count;
        public ICollection<string> Keys => _header.Keys;
        public ICollection<StringValues> Values => _header.Values;
    }
}
