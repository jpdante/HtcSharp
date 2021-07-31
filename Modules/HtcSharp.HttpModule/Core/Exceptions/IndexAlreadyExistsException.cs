using System;

namespace HtcSharp.HttpModule.Core.Exceptions {
    public class IndexAlreadyExistsException : Exception {

        public IndexAlreadyExistsException(string key) : base($"Index '{key}' already exists.") {

        }

    }
}