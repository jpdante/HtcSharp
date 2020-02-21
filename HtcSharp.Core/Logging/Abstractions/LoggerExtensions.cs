using System;

namespace HtcSharp.Core.Logging.Abstractions {
    public static class LoggerExtensions {

        public static void LogDebug(this ILogger logger, Type type, object obj, Exception ex) {
            logger.Log(LogLevel.Debug, type, obj, ex);
        }

        public static void LogInfo(this ILogger logger, Type type, object obj, Exception ex) {
            logger.Log(LogLevel.Info, type, obj, ex);
        }

        public static void LogWarn(this ILogger logger, Type type, object obj, Exception ex) {
            logger.Log(LogLevel.Warn, type, obj, ex);
        }

        public static void LogError(this ILogger logger, Type type, object obj, Exception ex) {
            logger.Log(LogLevel.Error, type, obj, ex);
        }

        public static void LogFatal(this ILogger logger, Type type, object obj, Exception ex) {
            logger.Log(LogLevel.Fatal, type, obj, ex);
        }

        public static void LogTrace(this ILogger logger, Type type, object obj, Exception ex) {
            logger.Log(LogLevel.Trace, type, obj, ex);
        }

    }
}