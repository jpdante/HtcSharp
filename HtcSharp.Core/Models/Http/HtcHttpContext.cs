using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Managers;

namespace HtcSharp.Core.Models.Http {
    public class HtcHttpContext {
        public HttpRequestContext Request { get; }
        public HttpResponseContext Response { get; }
        public HttpConnectionContext Connection { get; }
        public ErrorMessagesManager ErrorMessageManager { get; }

        public HtcHttpContext(HttpContext context, ErrorMessagesManager errorMessagesManager) {
            Request = new HttpRequestContext(context.Request);
            Response = new HttpResponseContext(context.Response);
            Connection = new HttpConnectionContext(context.Connection);
            ErrorMessageManager = errorMessagesManager;
        }
    }
}
