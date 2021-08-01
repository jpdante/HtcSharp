using System;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc {
    public class RequireSessionAttribute : Attribute {

        public readonly bool RequireSession;

        public RequireSessionAttribute(bool requireSession = true) {
            RequireSession = requireSession;
        }

    }
}