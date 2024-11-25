using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

#pragma warning disable CS8633
// Nullability of reference types in code targeting nullable reference types doesn't match the target settings
#pragma warning disable CS8767
// Nullability of reference types in type of parameter doesn't match the target delegate

namespace SuperTestWPF.Logger
{
    public class ListBoxLoggerProvider : ILoggerProvider
    {
        private readonly ObservableCollection<LogEntry> _logMessages;

        public ListBoxLoggerProvider(ObservableCollection<LogEntry> logMessages)
        {
            _logMessages = logMessages;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ListBoxLogger(_logMessages);
        }

        public void Dispose() { }

        private class ListBoxLogger : ILogger
        {
            private readonly ObservableCollection<LogEntry> _logMessages;

            public ListBoxLogger(ObservableCollection<LogEntry> logMessages)
            {
                _logMessages = logMessages;
            }

            public IDisposable? BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                string message = formatter(state, exception);
                _logMessages.Add(new LogEntry { Level = logLevel, Message = message });
            }
        }
    }
}
