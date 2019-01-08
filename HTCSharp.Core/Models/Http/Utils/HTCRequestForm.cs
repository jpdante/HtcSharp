using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestForm : IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable {
        private IFormCollection Form;
        private HTCRequestFormFiles FormFiles;

        public HTCRequestForm(IFormCollection form, HttpRequest httpRequest) {
            Form = form;
            FormFiles = new HTCRequestFormFiles(form.Files);
        }

        public HTCRequestFormFiles Files { get { return FormFiles; } }

        public string this[string key] {
            get {
                return Form[key];
            }
        }

        public bool ContainsKey(string key) {
            return Form.ContainsKey(key);
        }

        public bool TryGetValue(string key, out StringValues value) {
            return Form.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public int Count { get { return Form.Count; } }
        public ICollection<string> Keys { get { return Form.Keys; } }
    }
}
