using System;
using System.Collections.Generic;
using System.IO;
using HtcSharp.Shared.System;

namespace HtcSharp.Shared.IO {
    public static class PathExt {

        private static readonly string TempPath;
        private static readonly string ProgramDataPath;
        private static readonly string LogPath;
        private static readonly string ConfigPath;
        private static readonly string LockFilePath;
        private static readonly string CachePath;

        static PathExt() {
            TempPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "tmp", "htcsharp"),
                SystemOS.MacOSX => Path.Combine("/", "tmp", "htcsharp"),
                SystemOS.Windows => Path.Combine(Path.GetTempPath(), "htcsharp"),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "tmp", "htcsharp")
            };
            ProgramDataPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "share", "htcsharp"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Application Support", "htcsharp"),
                SystemOS.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "htcsharp"),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "share", "htcsharp")
            };
            LogPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "log", "htcsharp"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Application Support", "htcsharp"),
                SystemOS.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "htcsharp"),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "log", "htcsharp")
            };
            ConfigPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "etc", "htcsharp"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Preferences", "htcsharp"),
                SystemOS.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "htcsharp"),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "etc", "htcsharp")
            };
            LockFilePath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "lock", "htcsharp"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Application Support", "htcsharp"),
                SystemOS.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "htcsharp"),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "lock", "htcsharp")
            };
            CachePath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "cache", "htcsharp"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Caches", "htcsharp"),
                SystemOS.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "htcsharp"),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "cache", "htcsharp")
            };
        }

        public static void EnsureFolders() {
            if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
            if (!Directory.Exists(ProgramDataPath)) Directory.CreateDirectory(ProgramDataPath);
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
            if (!Directory.Exists(LockFilePath)) Directory.CreateDirectory(LockFilePath);
            if (!Directory.Exists(CachePath)) Directory.CreateDirectory(CachePath);
        }

        public static string GetTempPath() => TempPath;
        public static string GetTempPath(params string[] paths) => GetPath(TempPath, false, paths);
        public static string GetTempPath(bool create, params string[] paths) => GetPath(TempPath, create, paths);

        public static string GetProgramDataPath() => ProgramDataPath;
        public static string GetProgramDataPath(params string[] paths) => GetPath(ProgramDataPath, false, paths);
        public static string GetProgramDataPath(bool create, params string[] paths) => GetPath(ProgramDataPath, create, paths);

        public static string GetLogPath() => LogPath;
        public static string GetLogPath(params string[] paths) => GetPath(LogPath, false, paths);
        public static string GetLogPath(bool create, params string[] paths) => GetPath(LogPath, create, paths);

        public static string GetConfigPath() => ConfigPath;
        public static string GetConfigPath(params string[] paths) => GetPath(ConfigPath, false, paths);
        public static string GetConfigPath(bool create, params string[] paths) => GetPath(ConfigPath, create, paths);

        public static string GetLockFilePath() => LockFilePath;
        public static string GetLockFilePath(params string[] paths) => GetPath(LockFilePath, false, paths);
        public static string GetLockFilePath(bool create, params string[] paths) => GetPath(LockFilePath, create, paths);

        public static string GetCachePath() => CachePath;
        public static string GetCachePath(params string[] paths) => GetPath(CachePath, false, paths);
        public static string GetCachePath(bool create, params string[] paths) => GetPath(CachePath, create, paths);

        public static string GetPath(string basePath, bool create = false, params string[] paths) {
            var tmpPaths = new List<string>(paths);
            tmpPaths.Insert(0, basePath);
            string path = Path.Combine(tmpPaths.ToArray());
            if (create && !Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}