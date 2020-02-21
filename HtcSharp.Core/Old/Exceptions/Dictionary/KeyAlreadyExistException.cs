using System;

namespace HtcSharp.Core.Old.Exceptions.Dictionary {
    public class KeyAlreadyExistException : Exception {
        public readonly string Key;

        public KeyAlreadyExistException(string key) {
            Key = key;
        }

        public override string ToString() {
            return $"Key '{Key}' already exist.";
        }
    }
}
