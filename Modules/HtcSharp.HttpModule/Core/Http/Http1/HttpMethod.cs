namespace HtcSharp.HttpModule.Core.Http.Http1 {
    public enum HttpMethod : byte {
        Get,
        Put,
        Delete,
        Post,
        Head,
        Trace,
        Patch,
        Connect,
        Options,
        Custom,
        None = byte.MaxValue,
    }
}