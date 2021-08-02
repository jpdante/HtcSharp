using System;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc.Parsers {
    public class JsonObjectParser : IHttpObjectParser {

        public JsonSerializerOptions SerializerOptions { get; }

        public JsonObjectParser() {
            SerializerOptions = new JsonSerializerOptions();
        }

        public JsonObjectParser(JsonSerializerOptions options) {
            SerializerOptions = options;
        }

        public async Task<IHttpObject> Parse(HtcHttpContext httpContext, Type objectType) {
            return (IHttpObject) await JsonSerializer.DeserializeAsync(httpContext.Request.Body, objectType, SerializerOptions);
        }
    }
}