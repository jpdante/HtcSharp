namespace HtcSharp.HttpModule.Core.Http.Http {
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