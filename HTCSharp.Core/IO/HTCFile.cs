using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HTCSharp.Core.IO {
    public static class HTCFile {

        public static JObject GetJsonFile(string filename) {
            using (StreamReader file = File.OpenText(filename)) {
                using (JsonTextReader reader = new JsonTextReader(file)) {
                    JObject data = (JObject)JToken.ReadFrom(reader);
                    return data;
                }
            }
        }

        public static void CreateEmptyFile(string filename) {
            File.Create(filename);
        }

        public static string GetLastModified(string filename) {
            return File.GetLastWriteTime(filename).ToString("r");
        }

        public static string GetFileName(string path) {
            return Path.GetFileName(path);
        }

        public static string GetExtension(string filename) {
            return Path.GetExtension(filename);
        }

        public static bool Exists(string filename) {
            return File.Exists(filename);
        }

        public static FileInfo GetFileInfo(string filename) {
            return new FileInfo(filename);
        }

    }

    public class HTCFileBuffer : IDisposable {

        private string FilePath;
        private int BufferSize;
        private FileStream FileStream;
        private long StartRange = 0;
        private long EndRange = 0;
        private long CurrentByte;

        public HTCFileBuffer(string FilePath, int BufferSize) {
            this.FilePath = FilePath;
            this.BufferSize = BufferSize;
            this.FileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void CopyToStream(Stream StreamOut, long start, long end) {
            StartRange = start;
            EndRange = end;
            CopyToStream(StreamOut);
        }

        public void CopyToStream(Stream StreamOut) {
            this.FileStream.Position = StartRange;
            this.CurrentByte = StartRange;
            if (this.EndRange <= 0) this.EndRange = this.FileStream.Length;
            bool continueRead = true;
            byte[] buffer = new byte[BufferSize];
            int bytesRead;
            while (continueRead) {
                if ((EndRange - CurrentByte) > BufferSize) {
                    bytesRead = FileStream.Read(buffer, 0, buffer.Length);
                    CurrentByte += bytesRead;
                } else {
                    bytesRead = FileStream.Read(buffer, 0, (int)(EndRange - CurrentByte));
                    CurrentByte += bytesRead;
                }
                StreamOut.Write(buffer, 0, bytesRead);
                if (CurrentByte >= EndRange) continueRead = false;
            }
        }

        public string FileName { get { return this.FilePath; } }
        public int BufferLenght { get { return this.BufferSize; } }
        public long Lenght { get { return this.FileStream.Length; } }
        public Stream Stream { get { return this.FileStream; } }

        public void Dispose() {
            this.FileStream.Dispose();
            BufferSize = 0;
            FilePath = null;
            GC.SuppressFinalize(this);
        }
    }
}
