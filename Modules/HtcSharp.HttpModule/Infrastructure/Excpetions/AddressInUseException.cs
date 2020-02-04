using System;

namespace HtcSharp.HttpModule.Infrastructure.Excpetions {
    public class AddressInUseException : InvalidOperationException {
        public AddressInUseException(string message) : base(message) {
        }

        public AddressInUseException(string message, Exception inner) : base(message, inner) {
        }
    }
}