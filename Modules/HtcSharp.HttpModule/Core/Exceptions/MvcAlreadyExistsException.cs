using System;

namespace HtcSharp.HttpModule.Core.Exceptions {
    public class MvcAlreadyExistsException : Exception {

        public MvcAlreadyExistsException(string key) : base($"Mvc '{key}' already exists.") {

        }

    }
}