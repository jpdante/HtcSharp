using System.Collections.Generic;

namespace HtcSharp.Abstractions {
    public interface IModuleManager {
        
        public IEnumerable<IReadOnlyModule> Modules { get; }

        public bool HasModule(string name);

    }
}