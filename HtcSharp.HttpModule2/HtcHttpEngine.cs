using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HtcSharp.Core.Engines;
using HtcSharp.Core.Logging;

namespace HtcSharp.HttpModule2 {
    public class HtcHttpEngine : Engine {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public override void Start() {
            Logger.Debug("Starting...");
            Logger.Debug("Started!");
        }

        public override void Stop() {
            Logger.Debug("Stopping...");
            Logger.Debug("Stopped!");
        }

    }
}
