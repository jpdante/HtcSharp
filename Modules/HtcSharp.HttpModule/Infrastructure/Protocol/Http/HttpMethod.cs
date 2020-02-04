namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
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