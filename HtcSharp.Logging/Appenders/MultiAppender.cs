using System;
using System.Collections.Generic;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class MultiAppender : IAppender {

        private readonly List<IAppender> _appenders;

        public MultiAppender() {
            _appenders = new List<IAppender>();
        }

        public void Log(ILogger logger, LogLevel logLevel, object msg, params object[] objs) {
            foreach (var appender in _appenders) {
                try {
                    appender.Log(logger, logLevel, msg, objs);
                } catch {
                    // ignored
                }
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, object msg, Exception ex, params object[] objs) {
            foreach (var appender in _appenders) {
                try {
                    appender.Log(logger, logLevel, msg, ex, objs);
                } catch {
                    // ignored
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void AddAppender(IAppender appender) {
            _appenders.Add(appender);
        }

        public void RemoveAppender(IAppender appender) {
            _appenders.Remove(appender);
        }

        public void ClearLoggers() {
            _appenders.Clear();
        }

        public IReadOnlyList<IAppender> Appenders => _appenders;

        public void Dispose() {
            foreach (var appender in _appenders) {
                appender.Dispose();
            }
            _appenders.Clear();
        }
    }
}