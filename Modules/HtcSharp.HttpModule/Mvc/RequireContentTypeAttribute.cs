using System;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc {
    public class RequireContentTypeAttribute : Attribute {

        public readonly string ContentType;

        public RequireContentTypeAttribute(ContentType contentType) {
            ContentType = contentType.ToValue();
        }

        public RequireContentTypeAttribute(string contentType) {
            ContentType = contentType;
        }

    }
}