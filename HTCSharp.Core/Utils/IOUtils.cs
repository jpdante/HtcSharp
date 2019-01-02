using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HTCSharp.Core.Utils {
    public static class IOUtils {

        public static string[] GetFilesExceptionFix(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            List<String> files = new List<String>();
            foreach (string file in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly)) {
                files.Add(file);
            }
            if (searchOption == SearchOption.TopDirectoryOnly) return files.ToArray();
            foreach (string subDir in Directory.GetDirectories(path)) {
                try {
                    files.AddRange(GetFilesExceptionFix(subDir, searchPattern, searchOption));
                } catch { }
            }
            return files.ToArray();
        }

        public static JObject GetJsonFile(string filename) {
            using (StreamReader file = File.OpenText(filename)) {
                using (JsonTextReader reader = new JsonTextReader(file)) {
                    JObject data = (JObject)JToken.ReadFrom(reader);
                    return data;
                }
            }
        }

        public static string ReplacePathTags(string path) {
            return Path.GetFullPath((path
                .Replace("%workingpath%", Directory.GetCurrentDirectory())
                .Replace("%assemblypath%", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            ));
        }

    }
}
