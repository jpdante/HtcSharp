using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Engines {
    public abstract class Engine {

        protected JObject EngineConfig = null;

        public abstract void Start();

        public abstract void Stop();

        public void SetConfig(JObject config) {
            EngineConfig = config;
        }
    }
}
