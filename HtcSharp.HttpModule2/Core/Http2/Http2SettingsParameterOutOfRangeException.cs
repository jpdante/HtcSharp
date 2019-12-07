using System;

namespace HtcSharp.HttpModule2.Core.Http2 {
    internal sealed class Http2SettingsParameterOutOfRangeException : Exception {
        public Http2SettingsParameterOutOfRangeException(Http2SettingsParameter parameter, long lowerBound, long upperBound)
            : base($"HTTP/2 SETTINGS parameter {parameter} must be set to a value between {lowerBound} and {upperBound}") {
            Parameter = parameter;
        }

        public Http2SettingsParameter Parameter { get; }
    }
}