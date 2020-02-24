using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HtcSharp.Core.Utils {
    public static class HtcIOUtils {
        public static string ReplacePathTags(string path) {
            return Path.GetFullPath(path
                    .Replace("%WorkingPath%", Directory.GetCurrentDirectory())
                    .Replace("%AssemblyPath%", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                );
        }
    }
}
