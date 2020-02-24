using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Logging {
    
    // TODO: Fix this later

    /*public class ManagedFileLogger : ILogger {

        private FileLogger _currentLogger;
        private readonly ManagedFileLoggerConfig _config;

        public string WorkingPath { get; }
        public LogLevel MinLogLevel { get; set; }

        public ManagedFileLogger(string path, ManagedFileLoggerConfig config) : this(path, config, LogLevel.Trace) { }

        public ManagedFileLogger(string path, ManagedFileLoggerConfig config, LogLevel minLogLevel) {
            WorkingPath = path;
            _config = config;
            if (!Directory.Exists(WorkingPath)) {
                Directory.CreateDirectory(WorkingPath);
            }
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = $"{currentDate}.log";
            if (File.Exists(Path.Combine(WorkingPath, fileName))) {
                bool 
                for (var i = 1; i < _config.MaxFiles; i++) {
                    fileName = _config.CompressOldLogs ? $"{currentDate}-{i}.log.gz" : $"{currentDate}-{i}.log";
                    if (!File.Exists(fileName)) {

                    }
                }
            } else {

            }
        }

        public void Log(LogLevel logLevel, Type type, object obj, Exception ex) {
            CheckFile();
            _currentLogger?.Log(logLevel, type, obj, ex);
        }

        private void CheckFile() {
            if (_currentLogger.GetFileStream().Length > _config.MaxFileSize) {

            }
        }

        public bool IsEnabled(LogLevel logLevel) {
            return logLevel > MinLogLevel;
        }

        public void Dispose() {
            
        }

        public class ManagedFileLoggerConfig {

            public long MaxFileSize { get; set; } = 1024 * 1024 * 10;
            public int MaxFiles { get; set; } = 10;
            public bool CompressOldLogs { get; set; } = true;

        }
    }*/
}