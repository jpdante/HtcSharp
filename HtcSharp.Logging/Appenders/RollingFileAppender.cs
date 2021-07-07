using System;
using System.IO;
using System.IO.Compression;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class RollingFileAppender {

        private LogLevel _logLevels;
        private readonly RollingFileConfig _config;
        private readonly LogFormatter _logFormatter;
        private readonly object _lock;

        private string _currentFile;
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        public RollingFileAppender(RollingFileConfig config, LogLevel logLevels, LogFormatter logFormatter = null) {
            _config = config;
            _logLevels = logLevels;
            _logFormatter = logFormatter;
            _logFormatter ??= new LogFormatter();
            _lock = new object();
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            lock (_lock) {
                _streamWriter.Write(_logFormatter.FormatLog(logLevel, msg, objs));
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            lock (_lock) {
                _streamWriter.Write(_logFormatter.FormatLog(logLevel, msg, ex, objs));
            }
        }

        public bool IsEnabled(LogLevel logLevel) => _logLevels.HasFlag(logLevel);

        public void SetLogLevel(LogLevel logLevels) => _logLevels = logLevels;

        public void SetupFile() {
            if (_fileStream == null) {
                _currentFile = GetFileName();
            }
        }

        private string GetFileName() {
            return $".{_config.FileExtension}";
        }

        private void CompressFile(string logPath, string compressedPath) {
            using var logStream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var compressedStream = new FileStream(compressedPath, FileMode.Create, FileAccess.Write, FileShare.Write);
            using var zipArchive = new ZipArchive(compressedStream, ZipArchiveMode.Create);
            var entry = zipArchive.CreateEntry(Path.GetFileName(logPath), CompressionLevel.Optimal);
            using var entryStream = entry.Open();
            logStream.CopyTo(entryStream);
        }

        public void Dispose() {
            lock (_lock) {
                _streamWriter?.Dispose();
                _fileStream?.Dispose();
            }
        }

        public class RollingFileConfig {

            public long MaxFileSize { get; set; } = 16 * 1024 * 1024;

            public string FileFormat { get; set; } = "dd-mm-yyyy";

            public string FileExtension { get; set; } = "log";

            public bool CompressOldLogs { get; set; } = true;

        }
    }
}