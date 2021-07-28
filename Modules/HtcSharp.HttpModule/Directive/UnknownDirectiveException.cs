using System;

namespace HtcSharp.HttpModule.Directive {
    public class UnknownDirectiveException : Exception {

        public UnknownDirectiveException(string message) : base(message) {

        }

    }
}