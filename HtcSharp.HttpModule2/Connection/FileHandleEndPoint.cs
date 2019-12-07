using System;
using System.Net;

namespace HtcSharp.HttpModule2.Connection {
    public class FileHandleEndPoint : EndPoint {
        public FileHandleEndPoint(ulong fileHandle, FileHandleType fileHandleType) {
            FileHandle = fileHandle;
            FileHandleType = fileHandleType;

            switch (fileHandleType) {
                case FileHandleType.AUTO:
                case FileHandleType.TCP:
                case FileHandleType.PIPE:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public ulong FileHandle { get; }
        public FileHandleType FileHandleType { get; }
    }
}


