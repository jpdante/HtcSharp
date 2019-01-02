using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestHeaders {
        private IHeaderDictionary Header;

        public HTCRequestHeaders(IHeaderDictionary header) {
            Header = header;
        }

        public string this[string key] {
            get {
                return Header[key];
            }
        }

        public bool ConstainsKey(string key) {
            return Header.ContainsKey(key);
        }

        public long? ContentLength { get { return Header.ContentLength; } }
        public int Count { get { return Header.Count; } }
        public ICollection<string> Keys { get { return Header.Keys; } }
        public ICollection<StringValues> Values { get { return Header.Values; } }
    }
}
