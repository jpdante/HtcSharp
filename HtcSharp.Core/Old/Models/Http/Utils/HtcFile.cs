using System.Collections.Generic;
using System.IO;

namespace HtcSharp.Core.Old.Models.Http.Utils {
    public class HtcFile {
        private readonly IFormFile _file;

        public HtcFile(IFormFile file) {
            _file = file;
            Headers = new Dictionary<string, string>();
            foreach (string key in _file.Headers.Keys) {
                Headers.Add(key, _file.Headers[key]);
            }
        }

        public Dictionary<string, string> Headers { get; }
        public string ContentType => _file.ContentType;
        public string FileName => _file.FileName;
        public long Lenght => _file.Length;
        public string Name => _file.Name;
        public void CopyTo(Stream stream) => _file.CopyTo(stream);
    }
}
