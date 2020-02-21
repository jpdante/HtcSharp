using System;

namespace HtcSharp.Core.Old.Exceptions.Dictionary {
    public class KeyNotFoundException : Exception {
        public readonly string Key;

        public KeyNotFoundException(string key) {
            Key = key;
        }

        public override string ToString() {
            return $"Key '{Key}' does not exist.";
        }
    }
}
