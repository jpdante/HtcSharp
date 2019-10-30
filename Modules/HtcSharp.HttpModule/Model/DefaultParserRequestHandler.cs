using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Interface;

namespace HtcSharp.HttpModule.Model {
    public class DefaultParserRequestHandler : IParserRequestHandler {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public void OnRequestStart(HttpMethod httpMethod, HttpVersion httpVersion, Span<byte> targetBuffer, Span<byte> pathBuffer, Span<byte> query, Span<byte> customMethod, bool pathEncoded) {
            Logger.Debug($"Request => {httpMethod}; {httpVersion}; {pathEncoded};");
        }

        public void OnRequestHeader(Span<byte> nameBuffer, Span<byte> valueBuffer) {
            Logger.Debug($"Header =>");
        }

        public void OnRequestHeaderComplete() {
            Logger.Debug($"Header Completed =>");
        }
    }
}
