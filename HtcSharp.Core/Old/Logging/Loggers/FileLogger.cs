using System;
using System.IO;

namespace HtcSharp.Core.Old.Logging.Loggers {
    public class FileLogger : ILogger {

        private readonly FileStream _fileStream;
        private readonly StreamWriter _streamWriter;

        public FileLogger(string filename) {
            _fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            _streamWriter = new StreamWriter(_fileStream);
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
                _streamWriter.WriteLine($"[{time.ToString()}] [{logType}] [{type.Name}] {obj}");
            }
            if (ex != null) {
                _streamWriter.WriteLine($"[{time.ToString()}] [{logType}] [{type.Name}] {ex.Message} {ex.StackTrace}");
            }
            _streamWriter.Flush();
            _fileStream.Flush();
        }
    }
}
