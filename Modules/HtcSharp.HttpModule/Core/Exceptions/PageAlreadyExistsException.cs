using System;

namespace HtcSharp.HttpModule.Core.Exceptions {
    public class PageAlreadyExistsException : Exception {

        public PageAlreadyExistsException(string key) : base($"Page '{key}' already exists.") {

        }

    }
}