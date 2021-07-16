using System.Collections.Generic;
using HtcSharp.Logging.Config;

namespace HtcSharp.Core {
    public class Config {

        public string ModulesPath { get; set; } = "./modules/";

        public string PluginsPath { get; set; } = "./plugins/";

        public LoggingConfig Logging { get; set; } = new() {
            Enabled = true,
            Appender = new AppenderConfig {
                Type = AppenderType.Multi,
                Appenders = new List<AppenderConfig>() {
                    new AppenderConfig {
                        Type = AppenderType.Console,
                    },
                    new AppenderConfig {
                        Type = AppenderType.RollingFile,
                    }
                }
            }
        };

    }
}