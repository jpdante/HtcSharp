using System;
using System.Collections.Generic;

namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpEncodeDataException : HttpException {

        public Exception[] InnerExceptions;

        public HttpEncodeDataException(params Exception[] innerExceptions) : base(500, "Failed to encode body data.") {
            InnerExceptions = innerExceptions;
        }
    }
}