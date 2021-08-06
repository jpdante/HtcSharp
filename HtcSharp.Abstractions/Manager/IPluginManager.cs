using System.Collections.Generic;

namespace HtcSharp.Abstractions.Manager {
    public interface IPluginManager {
        
        public IEnumerable<IReadOnlyPlugin> Plugins { get; }

        public bool HasPlugin(string name);

    }
}