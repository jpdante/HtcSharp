using System;
using System.IO;

namespace HtcSharp.Core.Old.IO.Http {
    public class FileBuffer : IDisposable {
        private string _filePath;
        private int _bufferSize;
        private readonly FileStream _fileStream;
        private long _startRange = 0;
        private long _endRange = 0;
        private long _currentByte;

        public FileBuffer(string filePath, int bufferSize) {
            this._filePath = filePath;
            this._bufferSize = bufferSize;
            this._fileStream = new FileStream(this._filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void CopyToStream(Stream streamOut, long start, long end) {
            _startRange = start;
            _endRange = end;
            CopyToStream(streamOut);
        }

        public void CopyToStream(Stream streamOut) {
            this._fileStream.Position = _startRange;
            this._currentByte = _startRange;
            if (this._endRange <= 0) this._endRange = this._fileStream.Length;
            var continueRead = true;
            var buffer = new byte[_bufferSize];
            while (continueRead) {
                int bytesRead;
                if ((_endRange - _currentByte) > _bufferSize) {
                    bytesRead = _fileStream.Read(buffer, 0, buffer.Length);
                    _currentByte += bytesRead;
                } else {
                    bytesRead = _fileStream.Read(buffer, 0, (int)(_endRange - _currentByte));
                    _currentByte += bytesRead;
                }
                streamOut.Write(buffer, 0, bytesRead);
                if (_currentByte >= _endRange) continueRead = false;
            }
        }

        public string FileName => this._filePath;
        public int BufferLenght => this._bufferSize;
        public long Lenght => this._fileStream.Length;
        public Stream Stream => this._fileStream;

        public void Dispose() {
            this._fileStream.Dispose();
            _bufferSize = 0;
            _filePath = null;
            GC.SuppressFinalize(this);
        }
    }
}
