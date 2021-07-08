using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class RollingFileAppender {

        private LogLevel _logLevels;
        private readonly RollingFileConfig _config;
        private readonly LogFormatter _logFormatter;
        private readonly object _lock;

        private LogFile _currentLog;

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
                _currentLog.Writer.Write(_logFormatter.FormatLog(logLevel, msg, objs));
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            lock (_lock) {
                _currentLog.Writer.Write(_logFormatter.FormatLog(logLevel, msg, ex, objs));
            }
        }

        public bool IsEnabled(LogLevel logLevel) => _logLevels.HasFlag(logLevel);

        public void SetLogLevel(LogLevel logLevels) => _logLevels = logLevels;

        public void SetupFile() {
            if (_currentLog == null) {
                lock (_lock) {
                    var dateTime = DateTime.Now;
                    _currentLog = new LogFile(GetFileName(dateTime), dateTime);
                }
            } else {
                lock (_lock) {
                    if(!ShouldRotateFile(_currentLog)) return;
                    string oldLog = _currentLog.LogPath;
                    _currentLog.Dispose();
                    if (_config.CompressOldLogs) {
                        CompressFile(_currentLog.LogPath, Path.Combine(_config.Path, Path.GetFileNameWithoutExtension(oldLog) ?? throw new InvalidOperationException(), ".zip"));
                        if (File.Exists(_currentLog.LogPath)) File.Delete(_currentLog.LogPath);
                    }
                    var dateTime = DateTime.Now;
                    _currentLog = new LogFile(GetFileName(dateTime), dateTime);
                }
            }
        }

        private string GetFileName(DateTime dateTime, int count = 0) {
            var fileFormat = new StringBuilder(_config.FileFormat);
            fileFormat.Replace("%YYYY", dateTime.Year.ToString("0000"));
            fileFormat.Replace("%MM", dateTime.Month.ToString("00"));
            fileFormat.Replace("%DD", dateTime.Day.ToString("00"));
            fileFormat.Replace("%hh", dateTime.Hour.ToString("00"));
            fileFormat.Replace("%mm", dateTime.Minute.ToString("00"));
            fileFormat.Replace("%ss", dateTime.Second.ToString("00"));
            fileFormat.Replace("%ms", dateTime.Millisecond.ToString("00"));
            fileFormat.Replace("%tt", dateTime.Ticks.ToString("00"));

            string extra = count > 0 ? $".{count}" : "";

            var fileName = $"{fileFormat}{extra}.{_config.FileExtension}";
            return Path.Combine(_config.Path, fileName);
        }

        private bool ShouldRotateFile(LogFile currentLog) {
            if (_config.MaxFileSize != -1 && currentLog.FileStream.Length >= _config.MaxFileSize) return true;
            if ((currentLog.LogDate - DateTime.Now).TotalDays >= 1) return true;
            return false;
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
                _currentLog?.Dispose();
            }
        }

        private class LogFile : IDisposable {

            public FileStream FileStream { get; }

            public string LogPath { get; }

            public DateTime LogDate { get; }

            public StreamWriter Writer { get; }

            public LogFile(string path, DateTime dateTime) {
                LogPath = path;
                LogDate = dateTime;
                FileStream = new FileStream(LogPath, FileMode.Append, FileAccess.ReadWrite, FileShare.Read);
                Writer = new StreamWriter(FileStream);
            }

            public void Dispose() {
                Writer?.Dispose();
                FileStream?.Dispose();
            }
        }

        public class RollingFileConfig {

            public long MaxFileSize { get; set; } = 16 * 1024 * 1024;

            public string Path { get; set; } = null;

            public string FileFormat { get; set; } = "%DD-%MM-%YYYY";

            public string FileExtension { get; set; } = "log";

            public bool CompressOldLogs { get; set; } = true;

        }
    }
}