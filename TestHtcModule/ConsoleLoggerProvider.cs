using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace TestHtcModule {
    public class ConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope {

        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers;
        private IExternalScopeProvider _scopeProvider;

        public ConsoleLoggerProvider() {
            _loggers = new ConcurrentDictionary<string, ConsoleLogger>();
        }

        public ILogger CreateLogger(string categoryName) {
            return _loggers.GetOrAdd(categoryName, loggerName => new ConsoleLogger() {
                ScopeProvider = _scopeProvider
            });
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) {
            _scopeProvider = scopeProvider;
            foreach (var logger in _loggers) {
                logger.Value.ScopeProvider = _scopeProvider;
            }
        }
    }
}