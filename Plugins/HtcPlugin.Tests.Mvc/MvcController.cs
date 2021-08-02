using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc;
using Microsoft.AspNetCore.Http;

namespace HtcPlugin.Tests.Mvc {
    public static class MvcController {

        [HttpGet("/get")]
        public static async Task Get(HtcHttpContext httpContext) {
            await httpContext.Response.WriteAsync("Test data.");
        }

        public class JsonDataObj : IHttpJsonObject {

            [JsonPropertyName("data")]
            public string Data { get; set; }

            public Task<bool> ValidateData(HtcHttpContext httpContext) {
                return Task.FromResult(true);
            }
        }

        [HttpPost("/json-post")]
        [RequireContentType(ContentType.JSON)]
        public static async Task JsonPost(HtcHttpContext httpContext, JsonDataObj dataObj) {
            await httpContext.Response.WriteAsync(dataObj.Data ?? "Null");
        }

        public class XmlDataObj : IHttpXmlObject {

            public string Data { get; set; }

            public Task<bool> ValidateData(HtcHttpContext httpContext) {
                return Task.FromResult(true);
            }
        }

        [HttpPost("/xml-post")]
        [RequireContentType(ContentType.XML)]
        public static async Task XmlPost(HtcHttpContext httpContext, XmlDataObj dataObj) {
            await httpContext.Response.WriteAsync(dataObj.Data ?? "Null");
        }

        [HttpCustom("CUSTOM", "/xml-post")]
        public static async Task Custom(HtcHttpContext httpContext) {
            await httpContext.Response.WriteAsync("This is a custom method request.");
        }
    }
}
