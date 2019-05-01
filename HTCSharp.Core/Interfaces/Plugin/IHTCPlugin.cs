using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Interfaces.Plugin {
    public interface IHtcPlugin {
        string PluginName { get; }
        string PluginVersion { get; }
        void OnLoad();
        void OnEnable();
        void OnDisable();
    }
}
