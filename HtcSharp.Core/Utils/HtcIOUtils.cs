using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HtcSharp.Core.Utils {
    public static class HtcIoUtils {
        public static string ReplacePathTags(string path) {
            return Path.GetFullPath(path
                    .Replace("%workingpath%", Directory.GetCurrentDirectory())
                    .Replace("%assemblypath%", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                );
        }
    }
}
