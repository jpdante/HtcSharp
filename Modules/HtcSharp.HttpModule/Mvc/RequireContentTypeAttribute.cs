using System;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc {
    public class RequireContentTypeAttribute : Attribute {

        public ContentType ContentType;

        public RequireContentTypeAttribute(ContentType contentType = ContentType.DEFAULT) {
            ContentType = contentType;
        }

    }
}