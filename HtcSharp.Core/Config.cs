using System.Collections.Generic;
using HtcSharp.Logging.Config;
using HtcSharp.Logging.Config.Appenders;

namespace HtcSharp.Core {
    public class Config {

        public string ModulesPath { get; set; } = "./modules/";

        public string PluginsPath { get; set; } = "./plugins/";

        public LoggingConfig Logging { get; set; } = new() {
            Appender = new MultiAppenderConfig() {
                Appenders = new List<AppenderConfig>() {
                    new ConsoleAppenderConfig(),
                    new RollingAppenderConfig()
                }
            }
        };

    }
}