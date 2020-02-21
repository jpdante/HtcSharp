using System;

namespace HtcSharp.Core.Old.Logging {
    public class Logger {
        private readonly Type _objType;

        public Logger(Type type) {
            _objType = type;
        }

        public void Log(object obj) {
            Log(obj, null);
        }

        public void Log(object obj, Exception ex) {
            foreach(var logger in LogManager.GetLoggers()) {
                logger.Log(_objType, DateTime.Now, obj, ex);
            }
        }

        public void Debug(object obj) {
            Debug(obj, null);
        }

        public void Debug(object obj, Exception ex) {
            foreach (var logger in LogManager.GetLoggers()) {
                logger.Debug(_objType, DateTime.Now, obj, ex);
            }
        }

        public void Info(object obj) {
            Info(obj, null);
        }
        
        public void Info(object obj, Exception ex) {
            foreach (var logger in LogManager.GetLoggers()) {
                logger.Info(_objType, DateTime.Now, obj, ex);
            }
        }

        public void Warn(object obj) {
            Warn(obj, null);
        }

        public void Warn(object obj, Exception ex) {
            foreach (var logger in LogManager.GetLoggers()) {
                logger.Warn(_objType, DateTime.Now, obj, ex);
            }
        }

        public void Error(object obj) {
            Error(obj, null);
        }

        public void Error(object obj, Exception ex) {
            foreach (var logger in LogManager.GetLoggers()) {
                logger.Error(_objType, DateTime.Now, obj, ex);
            }
        }

        public void Fatal(object obj) {
            Fatal(obj, null);
        }

        public void Fatal(object obj, Exception ex) {
            foreach (var logger in LogManager.GetLoggers()) {
                logger.Fatal(_objType, DateTime.Now, obj, ex);
            }
        }

        public void Trace(object obj) {
            Trace(obj, null);
        }

        public void Trace(object obj, Exception ex) {
            foreach (var logger in LogManager.GetLoggers()) {
                logger.Trace(_objType, DateTime.Now, obj, ex);
            }
        }

    }
}
