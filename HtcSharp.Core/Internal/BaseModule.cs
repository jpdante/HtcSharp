using System.Runtime.Loader;
using HtcSharp.Abstractions;

namespace HtcSharp.Core.Internal {
    internal class BaseModule {

        public IModule Module { get; }

        private readonly AssemblyLoadContext _assemblyLoadContext;

        public BaseModule(IModule module, AssemblyLoadContext assemblyLoadContext) {
            Module = module;
            _assemblyLoadContext = assemblyLoadContext;
        }

        public void Unload() {
            _assemblyLoadContext.Unload();
        }

    }
}