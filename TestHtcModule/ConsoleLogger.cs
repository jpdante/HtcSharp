using System;
using Microsoft.Extensions.Logging;

namespace TestHtcModule {
    internal class ConsoleLogger : ILogger {

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {

            if (formatter == null) {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            Console.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? null;
    }
}
