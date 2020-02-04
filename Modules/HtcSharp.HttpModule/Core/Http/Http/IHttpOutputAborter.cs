namespace HtcSharp.HttpModule.Core.Http.Http {
    internal interface IHttpOutputAborter {
        void Abort(ConnectionAbortedException abortReason);
    }
}
