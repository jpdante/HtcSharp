using System.Reflection;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal;

namespace HtcSharp.Core.Plugin {
    internal class BasePlugin {

        public IPlugin Plugin { get; }

        internal Assembly Assembly { get; }

        internal CustomAssemblyLoadContext AssemblyLoadContext { get; }

        public BasePlugin(IPlugin plugin, Assembly assembly, CustomAssemblyLoadContext assemblyLoadContext) {
            Plugin = plugin;
            Assembly = assembly;
            AssemblyLoadContext = assemblyLoadContext;
        }

        public void Unload() {
            AssemblyLoadContext.Unload();
        }

    }
}