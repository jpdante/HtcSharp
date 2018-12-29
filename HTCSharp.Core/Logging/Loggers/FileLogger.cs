using HTCSharp.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTCSharp.Core.Logging.Loggers {
    public class FileLogger : ILogger {

        private FileStream fileStream;
        private StreamWriter streamWriter;

        public FileLogger(string filename) {
            if (!HTCFile.Exists(filename)) HTCFile.CreateEmptyFile(filename);
            fileStream = new FileStream(filename, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
            streamWriter = new StreamWriter(fileStream);
        }

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
                streamWriter.WriteLine($"[{time.ToString()}] [{type.Name}] [{logType}] {obj}");
            }
            if (ex != null) {
                streamWriter.WriteLine($"[{time.ToString()}] [{type.Name}] [{logType}] Message: {ex.Message}.{Environment.NewLine}StackTrace: {ex.StackTrace}");
            }
            streamWriter.Flush();
            fileStream.Flush();
        }
    }
}
