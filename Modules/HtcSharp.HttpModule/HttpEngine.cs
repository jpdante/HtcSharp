using System.Reflection;
using HtcSharp.Core.Engines;
using HtcSharp.Core.Logging;

namespace HtcSharp.HttpModule {
    public class HttpEngine : Engine {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public HttpEngine() {
        }

        public override void Start() {

        }

        public override void Stop() {
        }
    }
}