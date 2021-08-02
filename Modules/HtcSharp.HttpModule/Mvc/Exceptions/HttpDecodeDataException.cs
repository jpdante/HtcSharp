using System;
using System.Collections.Generic;

namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpDecodeDataException : HttpException {

        public Exception[] InnerExceptions;

        public HttpDecodeDataException(Exception[] innerExceptions) : base(500, "Failed to decode body data.") {
            InnerExceptions = innerExceptions;
        }
    }
}