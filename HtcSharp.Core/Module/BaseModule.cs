using System.Runtime.Loader;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal;

namespace HtcSharp.Core.Module {
    internal class BaseModule {

        public IModule Module { get; }

        private readonly CustomAssemblyLoadContext _assemblyLoadContext;

        public BaseModule(IModule module, CustomAssemblyLoadContext assemblyLoadContext) {
            Module = module;
            _assemblyLoadContext = assemblyLoadContext;
        }

        public void Unload() {
            _assemblyLoadContext.Unload();
        }

    }
}