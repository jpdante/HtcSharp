using System;

namespace HtcSharp.HttpModule.Core.Exceptions {
    public class MvcPageAlreadyExistsException : Exception {

        public MvcPageAlreadyExistsException(string key) : base($"Mvc page '{key}' already exists.") {

        }

    }
}