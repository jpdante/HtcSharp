using HTCSharp.Core.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Logging {
    public class ILog {

        private Type objType;

        public ILog(Type type) {
            objType = type;
        }

        public void Log(object obj) {
            Log(obj, null);
        }

        public void Log(object obj, Exception ex) {
            foreach(ILogger logger in LogManager.GetLoggers()) {
                logger.Log(objType, DateTime.Now, obj, ex);
            }
        }

        public void Debug(object obj) {
            Debug(obj, null);
        }

        public void Debug(object obj, Exception ex) {
            foreach (ILogger logger in LogManager.GetLoggers()) {
                logger.Debug(objType, DateTime.Now, obj, ex);
            }
        }

        public void Info(object obj) {
            Info(obj, null);
        }
        
        public void Info(object obj, Exception ex) {
            foreach (ILogger logger in LogManager.GetLoggers()) {
                logger.Info(objType, DateTime.Now, obj, ex);
            }
        }

        public void Warn(object obj) {
            Warn(obj, null);
        }

        public void Warn(object obj, Exception ex) {
            foreach (ILogger logger in LogManager.GetLoggers()) {
                logger.Warn(objType, DateTime.Now, obj, ex);
            }
        }

        public void Error(object obj) {
            Error(obj, null);
        }

        public void Error(object obj, Exception ex) {
            foreach (ILogger logger in LogManager.GetLoggers()) {
                logger.Error(objType, DateTime.Now, obj, ex);
            }
        }

        public void Fatal(object obj) {
            Fatal(obj, null);
        }

        public void Fatal(object obj, Exception ex) {
            foreach (ILogger logger in LogManager.GetLoggers()) {
                logger.Fatal(objType, DateTime.Now, obj, ex);
            }
        }

        public void Trace(object obj) {
            Trace(obj, null);
        }

        public void Trace(object obj, Exception ex) {
            foreach (ILogger logger in LogManager.GetLoggers()) {
                logger.Trace(objType, DateTime.Now, obj, ex);
            }
        }

    }
}
