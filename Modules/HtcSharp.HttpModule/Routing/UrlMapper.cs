using System.Collections.Generic;
using System.Reflection;

namespace HtcSharp.HttpModule.Routing {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public static class UrlMapper {
        internal static readonly List<string> IndexFiles = new List<string>();
        internal static readonly Dictionary<string, IHttpEvents> ExtensionPlugins = new Dictionary<string, IHttpEvents>();
        internal static readonly Dictionary<string, IHttpEvents> RegisteredPages = new Dictionary<string, IHttpEvents>();

        public static void RegisterIndexFile(string file) => IndexFiles.Add(file.ToLower());

        public static void UnRegisterIndexFile(string file) => IndexFiles.Remove(file.ToLower());

        public static void RegisterPluginExtension(string extension, IHttpEvents plugin) => ExtensionPlugins.Add(extension.ToLower(), plugin);

        public static void UnRegisterPluginExtension(string extension) => ExtensionPlugins.Remove(extension.ToLower());

        public static void RegisterPluginPage(string page, IHttpEvents plugin) => RegisteredPages.Add(page, plugin);

        public static void UnRegisterPluginPage(string page) => RegisteredPages.Remove(page);
    }
}