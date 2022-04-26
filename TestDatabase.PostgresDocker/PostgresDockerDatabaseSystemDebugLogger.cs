using System;
using Microsoft.Extensions.Logging;

namespace TestDatabase.PostgresDocker
{
    internal class PostgresDockerDatabaseSystemDebugLogger : ILogger<PostgresDockerDatabase>
    {
        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            System.Diagnostics.Debug.WriteLine(formatter.Invoke(state, exception));
        }
    }
}
