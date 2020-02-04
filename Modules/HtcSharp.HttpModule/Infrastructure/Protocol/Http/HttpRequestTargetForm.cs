namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    public enum HttpRequestTargetForm {
        Unknown = -1,
        // origin-form is the most common
        OriginForm,
        AbsoluteForm,
        AuthorityForm,
        AsteriskForm
    }
}
