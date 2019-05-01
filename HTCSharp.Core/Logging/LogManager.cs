using HtcSharp.Core.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Logging {
    public static class LogManager {

        private static readonly Dictionary<Type, Logger> TypeLoggers = new Dictionary<Type, Logger>();
        private static readonly List<ILogger> Loggers = new List<ILogger>();

        public static Logger GetILog(Type type) {
            var logger = new Logger(type);
            if(TypeLoggers.ContainsKey(type)) {
                return TypeLoggers[type];
            } else {
                TypeLoggers.Add(type, logger);
                return logger;
            }
        }

        public static void RegisterLogger(ILogger logger) {
            if (Loggers.Contains(logger)) return;
            else Loggers.Add(logger);
        }

        public static void UnRegisterLogger(ILogger logger) {
            if (Loggers.Contains(logger)) Loggers.Remove(logger);
            else return;
        }

        public static ILogger[] GetLoggers() {
            return Loggers.ToArray();
        }
    }
}
