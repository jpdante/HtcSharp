using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc;
using Microsoft.AspNetCore.Http;

namespace HtcPlugin.Tests.Mvc {
    public static class MvcController {

        [HttpGet("/test")]
        public static async Task Test(HtcHttpContext httpContext) {
            await httpContext.Response.WriteAsync("Test data.");
        }

        public class DataObj : IHttpJsonObject {

            [JsonPropertyName("data")]
            public string Data { get; set; }

            public Task<bool> ValidateData(HtcHttpContext httpContext) {
                return Task.FromResult(true);
            }
        }

        [HttpPost("/test")]
        [RequireContentType(ContentType.JSON)]
        public static async Task Test(HtcHttpContext httpContext, DataObj dataObj) {
            await httpContext.Response.WriteAsync(dataObj.Data ?? "Null");
        }
    }
}
