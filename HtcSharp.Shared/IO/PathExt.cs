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
                SystemOS.Unix => Path.Combine("/", "tmp"),
                SystemOS.MacOSX => Path.Combine("/", "tmp"),
                SystemOS.Windows => Path.GetTempPath(),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "tmp")
            };
            ProgramDataPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "share"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Application Support"),
                SystemOS.Windows => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "share")
            };
            LogPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "log"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Application Support"),
                SystemOS.Windows => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "log")
            };
            ConfigPath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "etc"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Preferences"),
                SystemOS.Windows => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "etc")
            };
            LockFilePath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "lock"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Application Support"),
                SystemOS.Windows => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "lock")
            };
            CachePath = EnvironmentExt.OperatingSystem switch {
                SystemOS.Unix => Path.Combine("/", "var", "cache"),
                SystemOS.MacOSX => Path.Combine("/", "Library", "Caches"),
                SystemOS.Windows => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _ => Path.Combine(Directory.GetCurrentDirectory(), "vfs", "var", "cache")
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