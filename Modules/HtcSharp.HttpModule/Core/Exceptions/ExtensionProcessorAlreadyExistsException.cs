using System;

namespace HtcSharp.HttpModule.Core.Exceptions {
    public class HttpEngineNotInitializedException : Exception {

        public HttpEngineNotInitializedException() : base("HttpEngine has not been initialized.") {

        }

    }
}