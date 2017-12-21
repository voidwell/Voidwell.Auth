using Microsoft.Extensions.Logging;

namespace Voidwell.Auth
{
    public class AuthLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerProvider _provider;

        public AuthLoggerFactory(LogLevel logLevel)
        {
            _provider = new AuthLoggingProvider(logLevel);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            return;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _provider.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}
