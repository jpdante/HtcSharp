using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HtcSharp.Core.Utils {
    public static class IoUtils {

        public static string[] GetFilesExceptionFix(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            var files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly).ToList();
            if (searchOption == SearchOption.TopDirectoryOnly) return files.ToArray();
            foreach (var subDir in Directory.GetDirectories(path)) {
                try {
                    files.AddRange(GetFilesExceptionFix(subDir, searchPattern, searchOption));
                } catch {
                    // ignored
                }
            }
            return files.ToArray();
        }

        public static JObject GetJsonFile(string filename) {
            using (var file = File.OpenText(filename)) {
                using (var reader = new JsonTextReader(file)) {
                    var data = (JObject)JToken.ReadFrom(reader);
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

        public static void CreateAspConfig(string path) {
            if(!File.Exists(path)) File.WriteAllText(path, "{\"Logging\":{\"IncludeScopes\":false,\"LogLevel\":{\"Default\":\"None\",\"System\":\"Warning\",\"Microsoft\":\"Warning\"}}}");
        }

        public static void CreateHtcConfig(string path) {
            if(!File.Exists(path)) File.WriteAllText(path, "{\"_PluginsPath\":\"\",\"Engines\":[{\"Engine\":\"http\",\"Config\":{\"Servers\":[{\"Hosts\":[\"0.0.0.0:80\"],\"Domain\":\"*:80\",\"Root\":\"%workingpath%/\",\"SSL\":false,\"Rewrites\":{}}]}}]}");
        }

    }
}
