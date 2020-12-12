using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Mvc;
using HtcSharp.HttpModule.Routing;

namespace HtcPlugin.Tests.Mvc {
    public static class MvcController {

        [HttpGet("/test")]
        public static async Task Test(HttpContext httpContext) {
            await httpContext.Response.WriteAsync("Test data.");
        }

        public class DataObj : IHttpJsonObject {

            [JsonPropertyName("data")]
            public string Data { get; set; }

            public Task<bool> ValidateData(HttpContext httpContext) {
                return Task.FromResult(true);
            }
        }

        [HttpPost("/test", ContentType.JSON)]
        public static async Task Test(HttpContext httpContext, DataObj dataObj) {
            await httpContext.Response.WriteAsync(dataObj.Data ?? "Null");
        }

    }
}
