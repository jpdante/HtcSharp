using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Logging.Loggers {
    public class ConsoleLogger : ILogger {
        public void Debug(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Debug", type, time, obj, ex);
        }

        public void Error(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Error", type, time, obj, ex);
        }

        public void Fatal(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Fatal", type, time, obj, ex);
        }

        public void Info(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Info", type, time, obj, ex);
        }

        public void Log(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Log", type, time, obj, ex);
        }

        public void Trace(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Trace", type, time, obj, ex);
        }

        public void Warn(Type type, DateTime time, object obj, Exception ex) {
            ExecuteLog("Warn", type, time, obj, ex);
        }

        public void ExecuteLog(string logType, Type type, DateTime time, object obj, Exception ex) {
            if (obj != null) {
                Console.WriteLine($"[{time.ToString()}] [{logType}] [{type.Name}] {obj}");
            }
            if (ex != null) {
                Console.WriteLine($"[{time.ToString()}] [{logType}] [{type.Name}] {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
