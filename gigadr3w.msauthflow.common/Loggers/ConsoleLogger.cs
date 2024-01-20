using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using gigadr3w.msauthflow.common.Configurations;

namespace gigadr3w.msauthflow.common.Loggers
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly LoggingConfiguration _configuration;
        public ConsoleLoggerProvider(LoggingConfiguration configuration)
            => _configuration = configuration;

        public ILogger CreateLogger(string categoryName)
            => new ConsoleLogger(_configuration);

        //disposes unmanaged resources
        public void Dispose() { }
    }

    public class ConsoleLogger : ILogger
    {
        private readonly LoggingConfiguration _configuration;

        public ConsoleLogger(LoggingConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
            => logLevel >= _configuration.Default;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"[{DateTime.Now.ToShortTimeString()}] - {eventId}";

            if (state != null)
            {
                message = string.Format("{0} - {1}", message, state.ToString());
            }

            if (exception != null)
            {
                message = string.Format("{0} - ** Exception {1} **", message, exception.ToString());
            }

            Console.WriteLine(message);
        }
    }
}
