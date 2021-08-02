using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc.Parsers {
    public class XmlObjectParser : IHttpObjectParser {

        public Task<IHttpObject> Parse(HtcHttpContext httpContext, Type objectType) {
            var xmlSerializer = new XmlSerializer(objectType);
            return Task.FromResult((IHttpObject) xmlSerializer.Deserialize(httpContext.Request.Body));
        }
    }
}