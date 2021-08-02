using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc.Parsers {
    public class XmlObjectParser : IHttpObjectParser {

        public Task<IHttpObject> Parse(HtcHttpContext httpContext, Type objectType) {
            // Failure to deserialize is a known C# bug
            // This is caused by the AssemblyLoadContext, there is no workaround for now, it is planned for .NET 6 Preview 2
            // https://github.com/dotnet/runtime/issues/1388
            // Possible solution: Use a custom XmlSerializer or external library?
            var xmlSerializer = new XmlSerializer(objectType);
            return Task.FromResult((IHttpObject) xmlSerializer.Deserialize(httpContext.Request.Body));
        }
    }
}