using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Interfaces.Plugin {
    public interface IHTCPlugin {
        string PluginName { get; }
        string PluginVersion { get; }
        void OnLoad();
        void OnEnable();
        void OnDisable();
    }
}
