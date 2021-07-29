using HtcSharp.Abstractions;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Mvc;

namespace HtcSharp.HttpModule {
    public static class PluginExtensions {

        public static void RegisterPage(this IPlugin plugin, string path, IHttpPage page) {
            HttpEngine.RegisterPage(plugin, path, page);
        }

        public static void RegisterMvc(this IPlugin plugin, HttpMvc mvc) {
            HttpEngine.RegisterMvc(plugin, mvc);
        }

        public static void RegisterExtensionProcessor(this IPlugin plugin, string extension, IExtensionProcessor extensionProcessor) {
            HttpEngine.RegisterExtensionProcessor(plugin, extension, extensionProcessor);
        }

    }
}