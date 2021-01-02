using System;
using Microsoft.Extensions.Logging;

namespace TestDatabase.SqlServerDocker
{
    public class SqlServerDockerDatabaseSystemDebugLogger : ILogger<SqlServerDockerDatabase>
    {
        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            System.Diagnostics.Debug.WriteLine(formatter.Invoke(state, exception));
        }
    }
}
