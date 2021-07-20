using System;

namespace HtcSharp.Shared.System {
    public static class EnvironmentExt {

        public static SystemOS OperatingSystem {
            get {
                switch (Environment.OSVersion.Platform) {
                    case PlatformID.Unix:
                        return SystemOS.Unix;
                    case PlatformID.MacOSX:
                        return SystemOS.MacOSX;
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return SystemOS.Windows;
                    default:
                        return SystemOS.Unknown;
                }
            }
        }

    }
}