using Microsoft.Extensions.Logging;

namespace gigadr3w.msauthflow.common.Loggers
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
            => new ConsoleLogger();

        //disposes unmanaged resources
        public void Dispose() { }
    }

    public class ConsoleLogger : ILogger
    {
        //return this class 
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"[{DateTime.Now.ToShortTimeString()}] - {eventId}";

            if (state != null)
            {
                message = string.Format("{0} - State {1}", message, state.ToString());
            }

            if (exception != null)
            {
                message = string.Format("{0} - ** Exception {1} **", message, exception.ToString());
            }

            Console.WriteLine(message);
        }
    }
}
