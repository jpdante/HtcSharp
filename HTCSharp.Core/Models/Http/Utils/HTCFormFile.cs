using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCFormFile {
        private IFormFile File;

        public HTCFormFile(IFormFile file) {
            File = file;
        }

        public HTCRequestHeaders Headers { get { return new HTCRequestHeaders(File.Headers); } }
        public string ContentType { get { return File.ContentType; } }
        public string FileName { get { return File.FileName; } }
        public long Lenght { get { return File.Length; } }
        public string Name { get { return File.Name; } }
        public void CopyTo(Stream stream) => File.CopyTo(stream);
    }
}
