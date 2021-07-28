using System;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpException : Exception {

        public readonly int Status;

        public HttpException(int status, string message) : base(message) {
            Status = status;
        }
    }
}
