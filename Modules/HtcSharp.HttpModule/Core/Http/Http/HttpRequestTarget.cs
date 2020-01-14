namespace HtcSharp.HttpModule.Core.Http.Http {
    internal enum HttpRequestTarget {
        Unknown = -1,
        // origin-form is the most common
        OriginForm,
        AbsoluteForm,
        AuthorityForm,
        AsteriskForm
    }
}
