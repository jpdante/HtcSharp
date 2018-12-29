using HTCSharp.Core.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Logging {
    public static class LogManager {

        private static Dictionary<Type, ILog> TypeLoggers = new Dictionary<Type, ILog>();
        private static List<ILogger> Loggers = new List<ILogger>();

        public static ILog GetILog(Type type) {
            if(TypeLoggers.ContainsKey(type)) {
                return TypeLoggers[type];
            } else {
                ILog logger = new ILog(type);
                TypeLoggers.Add(type, logger);
                return logger;
            }
        }

        public static void RegisterLogger(ILogger logger) {
            if (Loggers.Contains(logger)) return;
            else Loggers.Add(logger);
        }

        public static void UnregisterLogger(ILogger logger) {
            if (Loggers.Contains(logger)) Loggers.Remove(logger);
            else return;
        }

        public static ILogger[] GetLoggers() {
            return Loggers.ToArray();
        }
    }
}
