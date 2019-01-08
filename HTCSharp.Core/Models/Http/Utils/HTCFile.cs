using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCFile {
        private IFormFile File;

        public HTCFile(IFormFile file) {
            File = file;
            Headers = new Dictionary<string, string>();
            foreach (string key in File.Headers.Keys) {
                Headers.Add(key, File.Headers[key]);
            }
        }

        public Dictionary<string, string> Headers { get; }
        public string ContentType { get { return File.ContentType; } }
        public string FileName { get { return File.FileName; } }
        public long Lenght { get { return File.Length; } }
        public string Name { get { return File.Name; } }
        public void CopyTo(Stream stream) => File.CopyTo(stream);
    }
}
