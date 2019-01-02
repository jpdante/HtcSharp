using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestQuery {
        private IQueryCollection Query;

        public HTCRequestQuery(IQueryCollection query) {
            Query = query;
        }

        public string this[string key] {
            get {
                return Query[key];
            }
        }

        public bool ConstainsKey(string key) {
            return Query.ContainsKey(key);
        }

        public bool TryGetValue(string key, out StringValues value) {
            return Query.TryGetValue(key, out value);
        }

        public int Count { get { return Query.Count; } }
        public ICollection<string> Keys { get { return Query.Keys; } }
    }
}
