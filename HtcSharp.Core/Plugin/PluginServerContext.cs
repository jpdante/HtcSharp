using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Plugin {
    public class PluginServerContext {
        public string PluginsPath { get; }
        public PluginManager PluginManager { get; }

        public PluginServerContext(string pluginsPath, PluginManager pluginManager) {
            PluginsPath = pluginsPath;
            PluginManager = pluginManager;
        }
    }
}
