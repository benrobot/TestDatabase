using System;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace TestDatabase.PostgresDocker
{
    internal class ProgressMessageToILogger : IProgress<JSONMessage>
    {
        private readonly ILogger _logger;

        internal ProgressMessageToILogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Report(JSONMessage value) => _logger.LogDebug("Pulling: {0}", value.Status);
    }
}
