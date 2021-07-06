using System;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging {
    public static class LoggerExtensions {

        #region Debug

        public static void LogDebug(this ILogger logger, string message, params object[] objs) {
            logger.Log(LogLevel.Debug, message, objs);
        }

        public static void LogDebug(this ILogger logger, string message, Exception ex, params object[] objs) {
            logger.Log(LogLevel.Debug, message, ex, objs);
        }

        #endregion

        #region Info

        public static void LogInfo(this ILogger logger, string message, params object[] objs) {
            logger.Log(LogLevel.Info, message, objs);
        }

        public static void LogInfo(this ILogger logger, string message, Exception ex, params object[] objs) {
            logger.Log(LogLevel.Info, message, ex, objs);
        }

        #endregion

        #region Warn

        public static void LogWarn(this ILogger logger, string message, params object[] objs) {
            logger.Log(LogLevel.Warn, message, objs);
        }

        public static void LogWarn(this ILogger logger, string message, Exception ex, params object[] objs) {
            logger.Log(LogLevel.Warn, message, ex, objs);
        }

        #endregion

        #region Error

        public static void LogError(this ILogger logger, string message, params object[] objs) {
            logger.Log(LogLevel.Error, message, objs);
        }

        public static void LogError(this ILogger logger, string message, Exception ex, params object[] objs) {
            logger.Log(LogLevel.Error, message, ex, objs);
        }

        #endregion

        #region Fatal

        public static void LogFatal(this ILogger logger, string message, params object[] objs) {
            logger.Log(LogLevel.Fatal, message, objs);
        }

        public static void LogFatal(this ILogger logger, string message, Exception ex, params object[] objs) {
            logger.Log(LogLevel.Fatal, message, ex, objs);
        }

        #endregion
    }
}