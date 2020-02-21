using System;
using System.IO;
using System.Text;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Logging {
    public class FileLogger : FormattedLogger {

        private readonly FileStream _fileStream;
        private readonly StreamWriter _streamWriter;

        public string FileName { get; }
        public LogLevel MinLogLevel { get; set; }

        public FileLogger(string fileName) : this(fileName, LogLevel.Trace) { }

        public FileLogger(string fileName, LogLevel minLogLevel) {
            FileName = fileName;
            MinLogLevel = minLogLevel;
            _fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8);
        }

        public override void Log(LogLevel logLevel, Type type, object obj, Exception ex) {
            _streamWriter.WriteLine(FormatLog(logLevel, type, obj, ex));
            _streamWriter.Flush();
        }

        public override bool IsEnabled(LogLevel logLevel) {
            return logLevel > MinLogLevel;
        }

        public FileStream GetFileStream() => _fileStream;
        public StreamWriter GetStreamWriter() => _streamWriter;

        public override void Dispose() {
            _streamWriter.Flush();
            _fileStream.Flush();
            _streamWriter.Dispose();
            _fileStream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}