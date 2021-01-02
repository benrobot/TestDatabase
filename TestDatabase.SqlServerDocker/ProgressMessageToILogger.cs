using System;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace TestDatabase.SqlServerDocker
{
    public class ProgressMessageToILogger : IProgress<JSONMessage>
    {
        private readonly ILogger _logger;

        public ProgressMessageToILogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Report(JSONMessage value) => _logger.LogDebug("Pulling: {0}", value.Status);
    }
}
