using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Voidwell.Auth
{
    public class AuthLoggingProvider : ILoggerProvider
    {
        private readonly LogLevel _logLevel;
        private readonly ConcurrentDictionary<string, AuthConsoleLogger> _loggers = new ConcurrentDictionary<string, AuthConsoleLogger>();

        public AuthLoggingProvider(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new AuthConsoleLogger(name, _logLevel));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    public class AuthConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly LogLevel _logLevel;

        public AuthConsoleLogger(string name, LogLevel logLevel)
        {
            _name = name;
            _logLevel = logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _logLevel && !_name.StartsWith("Microsoft");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logEvent = formatter(state, exception);
            var Timestamp = DateTime.Now;

            Console.Write("[");
            WriteLogLevel(logLevel);
            Console.WriteLine($"{logLevel} {Timestamp:HH:mm:ss.fff}] {_name}\n{logEvent} {exception}");
        }

        private static void WriteLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.Write(logLevel);
            Console.ResetColor();
        }
    }
}
