using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SourceTools {
    public class Program {
        public static void Main(string[] args) {
            new Program().Run(args);
        }

        public void Run(string[] args) {
            string repositoryPath = GetHttpModuleDirectory(Directory.GetCurrentDirectory());
            ProcessRepository(repositoryPath);
        }

        public string GetHttpModuleDirectory(string path) {
            var directory = Directory.GetParent(path);
            return directory.Name.Equals("Modules") ? Path.Combine(directory.FullName, "HtcSharp.HttpModule") : GetHttpModuleDirectory(directory.FullName);
        }

        public void ProcessRepository(string repositoryPath) {
            foreach (string filePath in Directory.GetFiles(repositoryPath, "*.cs", SearchOption.AllDirectories)) {
                string relativePath = filePath.Replace(repositoryPath, "");
                if (relativePath.StartsWith(Path.DirectorySeparatorChar + "bin") || relativePath.StartsWith(Path.DirectorySeparatorChar + "obj")) continue;
                var fileProcessor = new FileProcessor(filePath, relativePath.Remove(0, 1));
                fileProcessor.ProcessFile();
                fileProcessor.Dispose();
            }
            FileProcessor.DisposeStatic();
        }
    }

    public class FileProcessor : IDisposable {
        private static readonly WebClient WebClient;
        private static readonly FileStream OutputStream;
        private static readonly StreamWriter OutputWriter;

        public FileStream FileStream;
        public StreamReader StreamReader;
        private bool _configured;
        private bool _processingConfig;

        private bool _ignoreCopyright;
        private bool _onlyWarnContentChange;
        private readonly string _localFile;
        private readonly string _localRelativeFile;
        private string _remoteFile;
        private int _remoteLine;
        private readonly List<int> _ignoreRemoteLines;
        private readonly List<int> _ignoreLocalLines;

        static FileProcessor() {
            WebClient = new WebClient();
            string logPath = Path.Combine(Directory.GetCurrentDirectory(), "error.log");
            if (File.Exists(logPath)) File.Delete(logPath);
            OutputStream = new FileStream(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            OutputWriter = new StreamWriter(OutputStream);
        }

        public FileProcessor(string filePath, string relativePath) {
            _localFile = filePath;
            _localRelativeFile = relativePath;
            FileStream = new FileStream(_localFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader = new StreamReader(FileStream);
            _ignoreCopyright = false;
            _onlyWarnContentChange = false;
            _ignoreRemoteLines = new List<int>();
            _ignoreLocalLines = new List<int>();
        }

        public void ProcessFile() {
            var firstLine = true;
            var missingCopyright = false;
            var localLineCount = 0;
            while (!StreamReader.EndOfStream) {
                string data = StreamReader.ReadLine();
                localLineCount++;
                if (data == null) continue;
                if (firstLine) {
                    if (!data.Contains("Copyright") && !data.Contains("License")) missingCopyright = true;
                    firstLine = false;
                }
                if (data.Contains("// SourceTools-Start", StringComparison.CurrentCultureIgnoreCase)) {
                    _processingConfig = true;
                } else if (data.Contains("// SourceTools-End", StringComparison.CurrentCultureIgnoreCase)) {
                    _processingConfig = false;
                    _configured = true;
                    break;
                } else {
                    if (_processingConfig) ProcessConfig(data);
                }
            }
            if (_processingConfig || !_configured) {
                Console.WriteLine($"[Warn] [No-SourceTools-Config] [{_localRelativeFile}]");
                return;
            }

            if (!_ignoreCopyright && missingCopyright) {
                Console.WriteLine($"[Error] [No-Copyright] [{_localRelativeFile}]");
                OutputWriter.WriteLine($"[Error] [No-Copyright] [{_localRelativeFile}]");
            }

            if (_remoteFile != null) {
                using var stringReader = new StringReader(_remoteFile.StartsWith("http") ? WebClient.DownloadString(_remoteFile) : File.ReadAllText(_remoteFile));

                string remoteLine;
                var remoteLineCount = 0;
                while ((remoteLine = stringReader.ReadLine()) != null) {
                    remoteLineCount++;
                    if (_ignoreRemoteLines.Contains(remoteLineCount)) continue;
                    if (remoteLineCount < _remoteLine) continue;
                    string localLine = StreamReader.ReadLine();
                    localLineCount++;
                    if (_ignoreLocalLines.Contains(localLineCount)) continue;
                    if (localLine == null || localLine.Equals(remoteLine, StringComparison.CurrentCultureIgnoreCase)) continue;
                    if (_onlyWarnContentChange) {
                        Console.WriteLine($"[Warn] [Content-Dont-Match] [{_localRelativeFile}] L {localLineCount} / R {remoteLineCount}");
                    } else {
                        Console.WriteLine($"[Error] [Content-Dont-Match] [{_localRelativeFile}] L {localLineCount} / R {remoteLineCount}");
                        OutputWriter.WriteLine($"[Error] [Content-Dont-Match] [{_localRelativeFile}] L {localLineCount} / R {remoteLineCount}");
                    }
                }
            }
        }

        private void ProcessConfig(string data) {
            data = data.Replace("// ", "");
            string[] dataSplit = data.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            switch (dataSplit[0].ToLower()) {
                case "remote-file":
                    _remoteFile = dataSplit[1];
                    break;
                case "start-at-remote-line":
                    _remoteLine = int.Parse(dataSplit[1]);
                    break;
                case "ignore-remote-line":
                    _ignoreRemoteLines.Add(int.Parse(dataSplit[1]));
                    break;
                case "ignore-local-line":
                    _ignoreLocalLines.Add(int.Parse(dataSplit[1]));
                    break;
                case "ignore-copyright":
                    _ignoreCopyright = true;
                    break;
                case "only-warn-content-change":
                    _onlyWarnContentChange = true;
                    break;
            }
        }

        public void Dispose() {
            FileStream?.Dispose();
            StreamReader?.Dispose();
        }

        public static void DisposeStatic() {
            OutputWriter?.Flush();
            OutputStream?.Flush();
            OutputWriter?.Close();
            OutputStream?.Close();
        }
    }
}