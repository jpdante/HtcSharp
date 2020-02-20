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
            if (exception == null) {
                Console.WriteLine($@"[{logLevel}] [{eventId.Id}] {message}");
            } else {
                Console.WriteLine($@"[{logLevel}] [{eventId.Id}] {message}");
                Console.WriteLine($@"{exception.Message}");
                Console.WriteLine($@"{exception.StackTrace}");
            }
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? null;
    }
}
