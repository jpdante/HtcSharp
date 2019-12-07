using System;
using HtcSharp.Core;
using HtcSharp.Core.Interfaces.Plugin;

namespace HtcSharp.HttpModule2 {
    public class HtcHttpModule : IHtcPlugin {
        public string PluginName => "HtcHttp";
        public string PluginVersion => "1.0.1";

        public void OnLoad() {
            HtcServer.Context.RegisterEngine("htc-http2", typeof(HtcHttpEngine));
        }

        public void OnEnable() {
            
        }

        public void OnDisable() {
            
        }
    }
}