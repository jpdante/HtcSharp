using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Appenders {
    public class RollingFileAppender : IAppender {

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
            SetupFile();
            lock (_lock) {
                _currentLog.Writer.Write(_logFormatter.FormatLog(logger, logLevel, msg, objs));
                _currentLog.Flush();
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            if (!_logLevels.HasFlag(logLevel)) return;
            SetupFile();
            lock (_lock) {
                _currentLog.Writer.Write(_logFormatter.FormatLog(logger, logLevel, msg, ex, objs));
                _currentLog.Flush();
            }
        }

        public bool IsEnabled(LogLevel logLevel) => _logLevels.HasFlag(logLevel);

        public void SetLogLevel(LogLevel logLevels) => _logLevels = logLevels;

        public void SetupFile() {
            if (_currentLog == null) {
                lock (_lock) {
                    var currentDate = DateTime.Now;
                    string path = GetFileName(currentDate, _config.FileExtension);
                    if (File.Exists(path) && _config.RollIfAlreadyExists) RollFile(path, currentDate);
                    _currentLog = new LogFile(path, currentDate);
                }
            } else {
                lock (_lock) {
                    if(!ShouldRollFile(_currentLog)) return;

                    string oldLogPath = _currentLog.LogPath;
                    var oldLogDate = _currentLog.LogDate;

                    _currentLog.Dispose();
                    _currentLog = null;

                    RollFile(oldLogPath, oldLogDate);

                    var dateTime = DateTime.Now;
                    _currentLog = new LogFile(GetFileName(dateTime, _config.FileExtension), dateTime);
                }
            }
        }

        private void RollFile(string logPath, DateTime dateTime) {
            if (_config.CompressOldLogs) {
                string rollPath = GetFileName(dateTime, "zip");
                if (File.Exists(rollPath)) File.Delete(rollPath);
                CompressFile(logPath, rollPath);
                AdvanceRollingFiles(rollPath);
            } else {
                AdvanceRollingFiles(logPath);
            }
        }

        public string GetFileName(DateTime dateTime, string extension) {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(_config.Name)) {
                builder.Append(_config.Name);
                builder.Append('.');
            }

            if (!string.IsNullOrEmpty(_config.FileFormat)) {
                builder.Append(dateTime.ToString(_config.FileFormat));
            }

            if (!string.IsNullOrEmpty(extension)) {
                builder.Append('.');
                builder.Append(extension);
            }

            return Path.Combine(_config.Path, builder.ToString());
        }

        private void AdvanceRollingFiles(string path) {
            for (uint i = _config.MaxBackupHistory; i > 0; i--) {
                var currentPath = $"{path}.{i}";
                if (!File.Exists(currentPath)) continue;
                if (i + 1 >= _config.MaxBackupHistory) {
                    File.Delete($"{path}.{i}");
                } else {
                    File.Move(currentPath, $"{path}.{i + 1}");
                }
            }
            File.Move(path, $"{path}.1");
        }

        private bool ShouldRollFile(LogFile currentLog) {
            if (_config.MaxFileSize != -1 && currentLog.FileStream.Length >= _config.MaxFileSize) return true;
            if (DateTime.Now - currentLog.LogDate >= _config.RollingSpan) return true;
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
                FileStream = new FileStream(LogPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                FileStream.Seek(FileStream.Length, SeekOrigin.Begin);
                Writer = new StreamWriter(FileStream);
            }

            public void Flush() {
                Writer?.Flush();
                FileStream?.Flush(true);
            }

            public void Dispose() {
                Writer?.Dispose();
                FileStream?.Dispose();
            }
        }

        public class RollingFileConfig {

            public string Path { get; set; } = Directory.GetCurrentDirectory();

            public string Name { get; set; } = "log";

            public string FileFormat { get; set; } = "dd-MM-yyyy";

            public string FileExtension { get; set; } = "log";

            public bool CompressOldLogs { get; set; } = true;

            public bool RollIfAlreadyExists { get; set; } = false;

            public long MaxFileSize { get; set; } = 16 * 1024 * 1024;

            public uint MaxBackupHistory { get; set; } = 10;

            public TimeSpan RollingSpan { get; set; } = new(1, 0, 0, 0);
        }
    }
}