using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestDatabase.PostgresDocker.Test.Helpers
{
    public class TestLoggerForPostgresDockerDatabase : ILogger<PostgresDockerDatabase>
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestLoggerForPostgresDockerDatabase(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _outputHelper.WriteLine(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
