using System;
using HtcSharp.Core;
using HtcSharp.Core.Interfaces.Plugin;

namespace HtcSharp.Http {
    public class HtcHttpEngine : IHtcPlugin {
        public string PluginName => "HtcHttp";
        public string PluginVersion => "1.0.0";

        public void OnLoad() {
            HtcServer.Context.RegisterEngine("HtcHttp", typeof(HttpEngine2));
        }

        public void OnEnable() {
            
        }

        public void OnDisable() {
            
        }
    }
}