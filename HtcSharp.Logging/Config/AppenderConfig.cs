using System;
using System.Collections.Generic;
using HtcSharp.Logging.Appenders;

namespace HtcSharp.Logging.Config {
    public sealed class AppenderConfig {

        public AppenderType Type { get; set; } = AppenderType.Null;

        public LogLevel LogLevel { get; set; } = LogLevel.All;

        public FormatterConfig Formatter { get; set; } = null;

        public Dictionary<string, string> Settings { get; set; } = null;

        public List<AppenderConfig> Appenders { get; set; } = null;

        public IAppender GetAppender() {
            var formatter = Formatter?.GetFormatter();
            switch (Type) {
                case AppenderType.Null:
                    return new NullAppender();
                case AppenderType.Multi:
                    var multiAppender = new MultiAppender();
                    if (Appenders == null) return multiAppender;
                    foreach (var appenderConfig in Appenders) {
                        multiAppender.AddAppender(appenderConfig.GetAppender());
                    }
                    return multiAppender;
                case AppenderType.Console:
                    return new ConsoleAppender(LogLevel, formatter);
                case AppenderType.File:
                    return Settings.TryGetValue("FileName", out string fileName) ? new FileAppender(fileName, LogLevel, formatter) : new FileAppender("log.log", LogLevel, formatter);
                case AppenderType.RollingFile:
                    var rollingConfig = new RollingFileAppender.RollingFileConfig();
                    if (Settings == null) return new RollingFileAppender(rollingConfig, LogLevel, formatter);
                    if (Settings.TryGetValue("Path", out string path)) rollingConfig.Path = path;
                    if (Settings.TryGetValue("Name", out string name)) rollingConfig.Name = name;
                    if (Settings.TryGetValue("FileFormat", out string fileFormat)) rollingConfig.FileFormat = fileFormat;
                    if (Settings.TryGetValue("FileExtension", out string fileExtension)) rollingConfig.FileExtension = fileExtension;
                    if (Settings.TryGetValue("CompressOldLogs", out string compressOldLogs)) rollingConfig.CompressOldLogs = bool.Parse(compressOldLogs);
                    if (Settings.TryGetValue("RollIfAlreadyExists", out string rollIfAlreadyExists)) rollingConfig.RollIfAlreadyExists = bool.Parse(rollIfAlreadyExists);
                    if (Settings.TryGetValue("MaxFileSize", out string maxFileSize)) rollingConfig.MaxFileSize = long.Parse(maxFileSize);
                    if (Settings.TryGetValue("MaxBackupHistory", out string maxBackupHistory)) rollingConfig.MaxBackupHistory = uint.Parse(maxBackupHistory);
                    if (Settings.TryGetValue("RollingSpan", out string rollingSpan)) rollingConfig.RollingSpan = TimeSpan.Parse(rollingSpan);
                    return new RollingFileAppender(rollingConfig, LogLevel, formatter);
                default:
                    return new NullAppender();
            }
        }
    }
}