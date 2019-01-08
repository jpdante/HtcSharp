using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestCookies {
        private IRequestCookieCollection Cookies;

        public HTCRequestCookies(IRequestCookieCollection cookies) {
            Cookies = cookies;
        }

        public string this[string key] {
            get {
                return Cookies[key];
            }
        }

        public bool ContainsKey(string key) {
            return Cookies.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value) {
            return Cookies.TryGetValue(key, out value);
        }

        public int Count { get { return Cookies.Count; } }
        public ICollection<string> Keys { get { return Cookies.Keys; } }
    }
}
