namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    internal interface IHttpOutputAborter {
        void Abort(ConnectionAbortedException abortReason);
    }
}
