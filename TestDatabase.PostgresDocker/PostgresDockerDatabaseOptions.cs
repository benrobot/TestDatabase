namespace TestDatabase.PostgresDocker
{
    /// <summary>
    /// Options for <see cref="PostgresDockerDatabase"/>. All options have reasonable defaults.
    /// </summary>
    public class PostgresDockerDatabaseOptions
    {
        /// <summary>
        /// Options for <see cref="PostgresDockerDatabase"/>. All options have reasonable defaults.
        /// </summary>
        /// <param name="dockerContainerName">Defaults to assembly name. Not enforced but recommend max length 30 characters and limited to alphanumerics and underscores.</param>
        /// <param name="dockerPostgresUser">Defaults to "postgres"</param>
        /// <param name="dockerPostgresPassword">Defaults to "mysecretpassword"</param>
        /// <param name="dockerPostgresHostPort">Defaults to 5432</param>
        /// <param name="dockerPostgresImageName">Defaults to "postgres"</param>
        /// <param name="dockerPostgresImageTag">Defaults to "16.4-alpine"</param>
        /// <param name="stopDockerInstanceOnDispose">Default true. Forced to true if remove docker container is true</param>
        /// <param name="removeDockerContainerOnDispose">If true then also stops docker instance</param>
        /// <param name="initialWaitForPostgresStartupInSeconds">Defaults to 10 seconds. The number of seconds to wait for SQL Server to startup before starting connection attempts</param>
        /// <param name="secondsToWaitBetweenPostgresConnectionRetries">Defaults to 5 seconds. The number of seconds to wait between retries when initially connecting to SQL Server</param>
        /// <param name="numberOfTimesToAttemptConnectingToPostgres">Defaults to 10 attempts. The number of attempts to try connecting to SQL Server before giving up</param>
        public PostgresDockerDatabaseOptions(
            string dockerContainerName = null,
            string dockerPostgresUser = "postgres",
            string dockerPostgresPassword = "mysecretpassword",
            int dockerPostgresHostPort = 5432,
            string dockerPostgresImageName = "postgres",
            string dockerPostgresImageTag = "16.4-alpine",
            bool? stopDockerInstanceOnDispose = null,
            bool? removeDockerContainerOnDispose = null,
            int initialWaitForPostgresStartupInSeconds = 10,
            int secondsToWaitBetweenPostgresConnectionRetries = 5,
            int numberOfTimesToAttemptConnectingToPostgres = 10
        )
        {
            if (dockerContainerName == null)
            {
                var executingAssemblyWithUnderscores = System.Reflection.Assembly.GetCallingAssembly().GetName()?.Name?
                                                           .Replace(".", "_")
                                                       ?? nameof(PostgresDockerDatabase);

                DockerContainerName = executingAssemblyWithUnderscores.Length > 30
                    ? executingAssemblyWithUnderscores.Substring(0, 30)
                    : executingAssemblyWithUnderscores;
            }
            else
            {
                DockerContainerName = dockerContainerName;
            }

            DockerPostgresUser = dockerPostgresUser;
            DockerPostgresPassword = dockerPostgresPassword;
            DockerPostgresHostPort = dockerPostgresHostPort;
            DockerPostgresImageName = dockerPostgresImageName;
            DockerPostgresImageTag = dockerPostgresImageTag;
            StopDockerInstanceOnDispose = stopDockerInstanceOnDispose == null || stopDockerInstanceOnDispose == true || removeDockerContainerOnDispose == true;
            RemoveDockerContainerOnDispose = removeDockerContainerOnDispose == true || (removeDockerContainerOnDispose == null && stopDockerInstanceOnDispose == null) || (removeDockerContainerOnDispose == null && stopDockerInstanceOnDispose == true);
            InitialWaitForPostgresStartupInSeconds = initialWaitForPostgresStartupInSeconds;
            SecondsToWaitBetweenPostgresConnectionRetries = secondsToWaitBetweenPostgresConnectionRetries;
            NumberOfTimesToAttemptConnectingToPostgres = numberOfTimesToAttemptConnectingToPostgres;
        }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerContainerName { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerPostgresUser { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerPostgresPassword { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int DockerPostgresHostPort { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerPostgresImageName { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerPostgresImageTag { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public bool StopDockerInstanceOnDispose { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public bool RemoveDockerContainerOnDispose { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int InitialWaitForPostgresStartupInSeconds { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int SecondsToWaitBetweenPostgresConnectionRetries { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int NumberOfTimesToAttemptConnectingToPostgres { get; }
    }
}
