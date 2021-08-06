using System.Collections.Generic;

namespace HtcSharp.Abstractions.Manager {
    public interface IModuleManager {
        
        public IEnumerable<IReadOnlyModule> Modules { get; }

        public bool HasModule(string name);

    }
}