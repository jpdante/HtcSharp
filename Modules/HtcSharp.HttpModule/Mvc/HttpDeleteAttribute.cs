namespace HtcSharp.HttpModule.Mvc {
    public class HttpDeleteAttribute : HttpMethodAttribute {

        public HttpDeleteAttribute(string path) : base("DELETE", path) { }

    }
}