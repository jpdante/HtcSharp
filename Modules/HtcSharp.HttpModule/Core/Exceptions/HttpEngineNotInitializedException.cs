using System;

namespace HtcSharp.HttpModule.Core.Exceptions {
    public class ExtensionProcessorAlreadyExistsException : Exception {

        public ExtensionProcessorAlreadyExistsException(string key) : base($"Extension processor '{key}' already exists.") {

        }

    }
}