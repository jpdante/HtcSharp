using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Old.Engines {
    public abstract class Engine {

        protected JObject EngineConfig = null;

        public abstract void Start();

        public abstract void Stop();

        public void SetConfig(JObject config) {
            EngineConfig = config;
        }
    }
}
