using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Pages {
    public class Default500PageMessage : IPageMessage {

        private const string Page = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><title>Listing of %RelativePath%</title><style>html{width:100%;height:100%;min-width:350px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:black;padding:20px}h1{margin:0 0 10px 0;word-wrap:break-word;text-align:center;font-size:40px}h2{margin:0 0 10px 0;word-wrap:break-word;text-align:center}span{margin-top:5px;display:block;text-align:center}bold{font-weight:bold}.red{color:#C20D2F}</style></head><body><div class=\"container\"><h1><color class=\"red\">500 - Internal Server Error</color></h1><h2>An internal server error occurred and it was not possible to process your request.</h2><h2>Please try again later.</h2> <span>Rendered by <bold>HtcSharp</bold></span></div></body></html>";

        public int StatusCode => 500;

        public Task<string> GetPageMessage(HttpContext httpContext) {
            return Task.FromResult(Page);
        }

        public async Task ExecutePageMessage(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCode;
            await httpContext.Response.WriteAsync(Page);
        }
    }
}
