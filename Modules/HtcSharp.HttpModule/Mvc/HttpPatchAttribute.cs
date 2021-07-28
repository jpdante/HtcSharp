namespace HtcSharp.HttpModule.Mvc {
    public class HttpPatchAttribute : HttpMethodAttribute {

        public HttpPatchAttribute(string path) : base("PATCH", path) { }

    }
}