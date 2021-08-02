using System;

namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpException : Exception {

        public readonly int Status;

        public HttpException(int status, string message) : base(message) {
            Status = status;
        }
    }
}
