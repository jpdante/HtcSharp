using System;
using System.IO;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class FileAppender : IAppender {

        private LogLevel _logLevels;
        private readonly LogFormatter _logFormatter;
        private readonly FileStream _fileStream;
        private readonly StreamWriter _streamWriter;
        private readonly object _lock;

        public FileAppender(string file, LogLevel logLevels, LogFormatter logFormatter = null) {
            _logLevels = logLevels;
            _logFormatter = logFormatter;
            _logFormatter ??= new LogFormatter();
            _fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _fileStream.Seek(_fileStream.Length, SeekOrigin.Begin);
            _streamWriter = new StreamWriter(_fileStream);
            _lock = new object();
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            lock (_lock) {
                _streamWriter.Write(_logFormatter.FormatLog(logger, logLevel, msg, objs));
                _streamWriter.Flush();
                _fileStream.Flush(true);
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            lock (_lock) {
                _streamWriter.Write(_logFormatter.FormatLog(logger, logLevel, msg, ex, objs));
                _streamWriter.Flush();
                _fileStream.Flush(true);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => _logLevels.HasFlag(logLevel);

        public void SetLogLevel(LogLevel logLevels) => _logLevels = logLevels;

        public void Dispose() {
            lock (_lock) {
                _streamWriter.Flush();
                _fileStream.Flush(true);
                _streamWriter?.Dispose();
                _fileStream?.Dispose();
            }
        }
    }
}