using System;
using System.IO;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class FileAppender : IAppender {

        private LogLevel _logLevels;
        private readonly IFormatter _logFormatter;
        private readonly FileStream _fileStream;
        private readonly StreamWriter _streamWriter;
        private readonly object _lock;

        public FileAppender(string fileName, LogLevel logLevels, IFormatter logFormatter = null) {
            _logLevels = logLevels;
            _logFormatter = logFormatter;
            _logFormatter ??= new Formatter();
            _fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _fileStream.Seek(_fileStream.Length, SeekOrigin.Begin);
            _streamWriter = new StreamWriter(_fileStream);
            _lock = new object();
        }

        public void Log(ILogger logger, LogLevel logLevel, object msg, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            lock (_lock) {
                _streamWriter.Write(_logFormatter.FormatLog(logger, logLevel, msg, null, objs));
                _streamWriter.Flush();
                _fileStream.Flush(true);
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, object msg, Exception ex, params object[] objs) {
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