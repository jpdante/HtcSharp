using System;

namespace HtcPlugin.Lua.Processor.Core {
    public class HeadersAlreadySentException : Exception {

        public HeadersAlreadySentException() : base("An attempt to change headers was done but they're already sent.") {

        }

    }
}