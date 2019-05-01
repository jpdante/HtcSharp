using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Models.Http.Utils {
    public class HtcRequestForm : IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable {
        private readonly IFormCollection _form;

        public HtcRequestForm(IFormCollection form, HttpRequest httpRequest) {
            _form = form;
            Files = new HtcRequestFormFiles(form.Files);
        }

        public HtcRequestFormFiles Files { get; }

        public string this[string key] => _form[key];

        public bool ContainsKey(string key) {
            return _form.ContainsKey(key);
        }

        public bool TryGetValue(string key, out StringValues value) {
            return _form.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() {
            return _form.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _form.GetEnumerator();
        }

        public int Count => _form.Count;
        public ICollection<string> Keys => _form.Keys;
    }
}
