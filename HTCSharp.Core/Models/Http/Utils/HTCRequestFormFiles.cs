using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCRequestFormFiles {
        private List<HTCFile> Files;

        public HTCRequestFormFiles(IFormFileCollection files) {
            Files = new List<HTCFile>();
            foreach (IFormFile file in files) {
                Files.Add(new HTCFile(file));
            }
        }

        public HTCFile this[int index] {
            get {
                return Files[index];
            }
        }

        public int Count { get { return Files.Count; } }
    }
}
