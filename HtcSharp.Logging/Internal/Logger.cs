﻿using System;

namespace HtcSharp.Logging.Internal {
    internal class Logger : ILogger {

        public Type Type { get; }

        public Logger(Type type) {
            Type = type;
        }

        public void Log(LogLevel logLevel, object msg, params object[] objs) {
            LoggerManager.DefaultAppender.Log(this, logLevel, msg, objs);
        }

        public void Log(LogLevel logLevel, object msg, Exception ex, params object[] objs) {
            if (ex == null) LoggerManager.DefaultAppender.Log(this, logLevel, msg, objs);
            else LoggerManager.DefaultAppender.Log(this, logLevel, msg, ex, objs);
        }

        public bool IsEnabled(LogLevel logLevel) => LoggerManager.DefaultAppender.IsEnabled(logLevel);
    }
}