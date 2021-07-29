using System.Reflection;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal;

namespace HtcSharp.Core.Module {
    internal class BaseModule {

        public IModule Module { get; }

        internal Assembly Assembly { get; }

        internal CustomAssemblyLoadContext AssemblyLoadContext { get; }

        public BaseModule(IModule module, Assembly assembly, CustomAssemblyLoadContext assemblyLoadContext) {
            Module = module;
            Assembly = assembly;
            AssemblyLoadContext = assemblyLoadContext;
        }

        public void Unload() {
            AssemblyLoadContext.Unload();
        }

    }
}