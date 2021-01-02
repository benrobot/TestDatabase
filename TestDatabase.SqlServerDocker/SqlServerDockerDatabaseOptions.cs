namespace TestDatabase.SqlServerDocker
{
    public class SqlServerDockerDatabaseOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dockerSqlServerPassword"></param>
        /// <param name="dockerSqlServerHostPort"></param>
        /// <param name="dockerSqlServerImageName"></param>
        /// <param name="dockerSqlServerImageTag"></param>
        /// <param name="dockerContainerName">Not enforced but recommend max length 30 characters and limited to alphanumerics and underscores.</param>
        /// <param name="stopDockerInstanceOnDispose">Default true. Forced to true if remove docker container is true</param>
        /// <param name="removeDockerContainerOnDispose">If true then also stops docker instance</param>
        public SqlServerDockerDatabaseOptions(
            string dockerContainerName = null,
            string dockerSqlServerPassword = "yourStrong(!)Password",
            int dockerSqlServerHostPort = 1433,
            string dockerSqlServerImageName = "mcr.microsoft.com/mssql/server",
            string dockerSqlServerImageTag = "2019-GA-ubuntu-16.04",
            bool? stopDockerInstanceOnDispose = null,
            bool? removeDockerContainerOnDispose = null
        )
        {
            if (dockerContainerName == null)
            {
                var executingAssemblyWithUnderscores = System.Reflection.Assembly.GetExecutingAssembly().GetName()?.Name?
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
        }

        public string DockerContainerName { get; }
        public string DockerSqlServerPassword { get; }
        public int DockerSqlServerHostPort { get; }
        public string DockerSqlServerImageName { get; }
        public string DockerSqlServerImageTag { get; }
        public bool StopDockerInstanceOnDispose { get; }
        public bool RemoveDockerContainerOnDispose { get; }
    }
}
