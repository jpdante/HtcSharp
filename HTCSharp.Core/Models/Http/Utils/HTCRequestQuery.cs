using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestQuery {
        private Dictionary<string, string> Query;

        public HTCRequestQuery(IQueryCollection query) {
            Query = new Dictionary<string, string>();
            foreach(var row in query) {
                Query.Add(row.Key, row.Value);
            }
        }

        public string this[string key] {
            get {
                return Query[key];
            }
        }

        public void Inject(string key, string value) {
            Query.Add(key, value);
        }

        public bool ContainsKey(string key) {
            return Query.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value) {
            return Query.TryGetValue(key, out value);
        }

        public int Count { get { return Query.Count; } }
        public ICollection<string> Keys { get { return Query.Keys; } }
    }
}
