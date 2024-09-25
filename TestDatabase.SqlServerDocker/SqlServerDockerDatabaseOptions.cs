namespace TestDatabase.SqlServerDocker
{
    /// <summary>
    /// Options for <see cref="SqlServerDockerDatabase"/>. All options have reasonable defaults.
    /// </summary>
    public class SqlServerDockerDatabaseOptions
    {
        /// <summary>
        /// Options for <see cref="SqlServerDockerDatabase"/>. All options have reasonable defaults.
        /// </summary>
        /// <param name="dockerSqlServerPassword">Defaults to "yourStrong(!)Password"</param>
        /// <param name="dockerSqlServerHostPort">Defaults to 1433</param>
        /// <param name="dockerSqlServerImageName">Defaults to "mcr.microsoft.com/mssql/server"</param>
        /// <param name="dockerSqlServerImageTag">Defaults to "2022-CU14-ubuntu-22.04"</param>
        /// <param name="dockerContainerName">Defaults to assembly name. Not enforced but recommend max length 30 characters and limited to alphanumerics and underscores.</param>
        /// <param name="stopDockerInstanceOnDispose">Default true. Forced to true if remove docker container is true</param>
        /// <param name="removeDockerContainerOnDispose">If true then also stops docker instance</param>
        /// <param name="initialWaitForSqlServerStartupInSeconds">Defaults to 10 seconds. The number of seconds to wait for SQL Server to startup before starting connection attempts</param>
        /// <param name="secondsToWaitBetweenSqlServerConnectionRetries">Defaults to 5 seconds. The number of seconds to wait between retries when initially connecting to SQL Server</param>
        /// <param name="numberOfTimesToAttemptConnectingToSqlServer">Defaults to 10 attempts. The number of attempts to try connecting to SQL Server before giving up</param>
        public SqlServerDockerDatabaseOptions(
            string dockerContainerName = null,
            string dockerSqlServerPassword = "yourStrong(!)Password",
            int dockerSqlServerHostPort = 1433,
            string dockerSqlServerImageName = "mcr.microsoft.com/mssql/server",
            string dockerSqlServerImageTag = "2022-CU14-ubuntu-22.04",
            bool? stopDockerInstanceOnDispose = null,
            bool? removeDockerContainerOnDispose = null,
            int initialWaitForSqlServerStartupInSeconds = 10,
            int secondsToWaitBetweenSqlServerConnectionRetries = 5,
            int numberOfTimesToAttemptConnectingToSqlServer = 10
        )
        {
            if (dockerContainerName == null)
            {
                var executingAssemblyWithUnderscores = System.Reflection.Assembly.GetCallingAssembly().GetName()?.Name?
                                                           .Replace(".", "_")
                                                       ?? nameof(SqlServerDockerDatabase);

                DockerContainerName = executingAssemblyWithUnderscores.Length > 30
                    ? executingAssemblyWithUnderscores.Substring(0, 30)
                    : executingAssemblyWithUnderscores;
            }
            else
            {
                DockerContainerName = dockerContainerName;
            }

            DockerSqlServerPassword = dockerSqlServerPassword;
            DockerSqlServerHostPort = dockerSqlServerHostPort;
            DockerSqlServerImageName = dockerSqlServerImageName;
            DockerSqlServerImageTag = dockerSqlServerImageTag;
            StopDockerInstanceOnDispose = stopDockerInstanceOnDispose == null || stopDockerInstanceOnDispose == true || removeDockerContainerOnDispose == true;
            RemoveDockerContainerOnDispose = removeDockerContainerOnDispose == true || (removeDockerContainerOnDispose == null && stopDockerInstanceOnDispose == null) || (removeDockerContainerOnDispose == null && stopDockerInstanceOnDispose == true);
            InitialWaitForSqlServerStartupInSeconds = initialWaitForSqlServerStartupInSeconds;
            SecondsToWaitBetweenSqlServerConnectionRetries = secondsToWaitBetweenSqlServerConnectionRetries;
            NumberOfTimesToAttemptConnectingToSqlServer = numberOfTimesToAttemptConnectingToSqlServer;
        }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerContainerName { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerSqlServerPassword { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int DockerSqlServerHostPort { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerSqlServerImageName { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public string DockerSqlServerImageTag { get; }

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
        public int InitialWaitForSqlServerStartupInSeconds { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int SecondsToWaitBetweenSqlServerConnectionRetries { get; }

        /// <summary>
        /// Use constructor to set this option
        /// </summary>
        public int NumberOfTimesToAttemptConnectingToSqlServer { get; }
    }
}
