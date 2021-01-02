using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestDatabase.SqlServerDocker.Test.Helpers
{
    public class TestLoggerForSqlServerDockerDatabase : ILogger<SqlServerDockerDatabase>
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestLoggerForSqlServerDockerDatabase(ITestOutputHelper outputHelper)
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
