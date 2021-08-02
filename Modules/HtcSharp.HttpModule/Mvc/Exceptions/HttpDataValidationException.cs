using System;
using System.Collections.Generic;

namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpDataValidationException : HttpException {

        public HttpDataValidationException() : base(400, "Data validation failed.") {
 
        }
    }
}