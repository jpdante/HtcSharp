using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTCSharp.Core.IO {
    public static class HTCDirectory {

        public static bool Exists(string path) {
            return Directory.Exists(path);
        }

        public static string CombinePath(string path1, string path2) {
            return Path.Combine(path1, path2);
        }

        public static string[] GetFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            List<String> files = new List<String>();
            foreach (string file in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly)) {
                files.Add(file);
            }
            if (searchOption == SearchOption.TopDirectoryOnly) return files.ToArray();
            foreach (string subDir in Directory.GetDirectories(path)) {
                try {
                    files.AddRange(GetFiles(subDir, searchPattern, searchOption));
                } catch { }
            }
            return files.ToArray();
        }

        public static string PathFromFileName(string path) {
            return Path.GetDirectoryName(path);
        }

        public static string GetFileName(string path) {
            return Path.GetFileName(path);
        }

        public static string[] GetDirectories(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            return Directory.GetDirectories(path, searchPattern, searchOption);
        }

        public static string GetWorkingDirectory() {
            return Directory.GetCurrentDirectory();
        }

    }
}
