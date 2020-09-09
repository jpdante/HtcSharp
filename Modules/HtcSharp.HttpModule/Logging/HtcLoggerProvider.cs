using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Logging;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Logging {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    class HtcLoggerProvider : ILoggerProvider, ISupportExternalScope {
        private readonly ConcurrentDictionary<string, HtcLogger> _loggers;
        private IExternalScopeProvider _scopeProvider;
        private readonly HtcSharp.Core.Logging.Abstractions.ILogger _logger;

        public HtcLoggerProvider(HtcSharp.Core.Logging.Abstractions.ILogger logger) {
            _logger = logger;
            _loggers = new ConcurrentDictionary<string, HtcLogger>();
        }

        public ILogger CreateLogger(string categoryName) {
            return _loggers.GetOrAdd(categoryName, loggerName => new HtcLogger(_logger) {ScopeProvider = _scopeProvider});
        }

        public void Dispose() {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) {
            _scopeProvider = scopeProvider;
            foreach (var logger in _loggers) {
                logger.Value.ScopeProvider = _scopeProvider;
            }
        }
    }
}