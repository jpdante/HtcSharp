using System.Runtime.Loader;
using HtcSharp.Abstractions;

namespace HtcSharp.Core.Plugin {
    internal class BasePlugin {

        public IPlugin Plugin { get; }

        private readonly AssemblyLoadContext _assemblyLoadContext;

        public BasePlugin(IPlugin plugin, AssemblyLoadContext assemblyLoadContext) {
            Plugin = plugin;
            _assemblyLoadContext = assemblyLoadContext;
        }

        public void Unload() {
            _assemblyLoadContext.Unload();
        }

    }
}