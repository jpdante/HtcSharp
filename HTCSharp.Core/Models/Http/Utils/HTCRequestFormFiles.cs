using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestFormFiles {
        private List<HTCFormFile> Files;

        public HTCRequestFormFiles(IFormFileCollection files) {
            Files = new List<HTCFormFile>();
            foreach (IFormFile file in files) {
                Files.Add(new HTCFormFile(file));
            }
        }

        public HTCFormFile this[int index] {
            get {
                return Files[index];
            }
        }

        public int Count { get { return Files.Count; } }
    }
}
