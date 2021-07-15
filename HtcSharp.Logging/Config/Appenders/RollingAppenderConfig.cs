using System;
using System.IO;
using HtcSharp.Logging.Appenders;

namespace HtcSharp.Logging.Config.Appenders {
    public class RollingAppenderConfig : AppenderConfig {

        public override AppenderType Type { get; set; } = AppenderType.Rolling;

        public string Path { get; set; } = Directory.GetCurrentDirectory();

        public string Name { get; set; } = "log";

        public string FileFormat { get; set; } = "dd-MM-yyyy";

        public string FileExtension { get; set; } = "log";

        public bool CompressOldLogs { get; set; } = true;

        public bool RollIfAlreadyExists { get; set; } = false;

        public long MaxFileSize { get; set; } = 16 * 1024 * 1024;

        public uint MaxBackupHistory { get; set; } = 10;

        public TimeSpan RollingSpan { get; set; } = new(1, 0, 0, 0);

        public override IAppender GetAppender() {
            var formatter = Formatter?.GetFormatter();
            return new RollingFileAppender(new RollingFileAppender.RollingFileConfig {
                Path = Path,
                Name = Name,
                FileFormat = FileFormat,
                FileExtension = FileExtension,
                CompressOldLogs = CompressOldLogs,
                RollIfAlreadyExists = RollIfAlreadyExists,
                MaxFileSize = MaxFileSize,
                MaxBackupHistory = MaxBackupHistory,
                RollingSpan = RollingSpan
            }, LogLevel, formatter);
        }
    }
}