using System;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc {
    public class RequireSessionAttribute : Attribute {

        public bool RequireSession;

        public RequireSessionAttribute(bool requireSession = true) {
            RequireSession = requireSession;
        }

    }
}