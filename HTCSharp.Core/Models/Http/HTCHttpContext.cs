using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http {
    public class HTCHttpContext {

        public HttpRequestContext Request { get; }
        public HttpResponseContext Response { get; }
        public HttpConnectionContext Connection { get; }

        public HTCHttpContext(HttpContext context) {
            Request = new HttpRequestContext(context.Request);
            Response = new HttpResponseContext(context.Response);
            Connection = new HttpConnectionContext(context.Connection);
        }
    }
}
