using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core {
    public static class HtcVersion {
        public static int Major = 1;
        public static int Minor = 1;
        public static int Patch = 1;
        public static int Build = 1;

        public static string GetVersion() {
            return $"{Major}.{Minor}.{Patch}.{Build}";
        }
    }
}
