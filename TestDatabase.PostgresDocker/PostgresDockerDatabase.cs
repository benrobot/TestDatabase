using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestDatabase.Abstractions;

namespace TestDatabase.PostgresDocker
{
    /// <summary>
    /// Starts a docker container running SQL Server and blocks until SQL Server is ready to accept connections
    /// </summary>
    public class PostgresDockerDatabase : ITestDatabase
    {
        private readonly ILogger<PostgresDockerDatabase> _logger;
        private readonly PostgresDockerDatabaseOptions _options;

        private string _dockerContainerId;

        /// <summary>
        /// Starts a docker container running SQL Server and blocks until SQL Server is ready to accept connections
        /// </summary>
        /// <param name="options">Example new PostgresDockerDatabaseOptions()</param>
        /// <param name="logger">Optional. If not provided then logging will be performed using <see cref="System.Diagnostics.Debug"/></param>
        public PostgresDockerDatabase(PostgresDockerDatabaseOptions options, ILogger<PostgresDockerDatabase> logger = null)
        {
            _logger = logger ?? new PostgresDockerDatabaseSystemDebugLogger();
            _options = options;
            StartDockerInstanceAsync().Wait();
            WaitForServerToStartAsync().Wait();
        }

        /// <summary>
        /// Returns "Data Source=tcp:localhost,{_options.DockerPostgresHostPort};User Id={_options.DockerPostgresUser};Password={_options.DockerPostgresPassword};"
        /// </summary>
        /// <returns>Connection string</returns>
        public string GetConnectionString() => $"Host=localhost;Port={_options.DockerPostgresHostPort};Username={_options.DockerPostgresUser};Password={_options.DockerPostgresPassword};";

        /// <summary>
        /// Returns "Data Source=tcp:localhost,{_options.DockerPostgresHostPort};User Id={_options.DockerPostgresUser};Password={_options.DockerPostgresPassword};Initial Catalog={databaseName}"
        /// </summary>
        /// <param name="databaseName">Database name to use a Initial Catalog in connection string</param>
        /// <returns>Connection string</returns>
        public string GetConnectionString(string databaseName) =>
            $"{GetConnectionString()};Database={databaseName}";

        /// <summary>
        /// Creates and opens a connection using the connection string from <see cref="GetConnectionString()"/>
        /// </summary>
        /// <returns>New open connection</returns>
        public IDbConnection GetNewDbConnection()
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Creates and opens a connection using the connection string from <see cref="GetConnectionString(string)"/>
        /// </summary>
        /// <param name="databaseName">Database name to connect to</param>
        /// <returns>New open connection</returns>
        public IDbConnection GetNewDbConnection(string databaseName)
        {
            var conn = new NpgsqlConnection(GetConnectionString(databaseName));
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Stops and/or removes docker container based on options provided to <see cref="PostgresDockerDatabase(PostgresDockerDatabaseOptions, ILogger{PostgresDockerDatabase})"/>
        /// </summary>
        public void Dispose()
        {
            if (_options.StopDockerInstanceOnDispose)
            {
                StopDockerInstanceAsync().Wait();
            }

            if (_options.RemoveDockerContainerOnDispose)
            {
                RemoveDockerContainerAsync().Wait();
            }

            GC.SuppressFinalize(this);
        }

        private async Task StartDockerInstanceAsync()
        {
            using var dockerClient = new DockerClientConfiguration()
                .CreateClient();

            _logger.LogDebug($"Looking for existing container named {_options.DockerContainerName}");
            var existingContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"name", new Dictionary<string, bool> {{ _options.DockerContainerName, true}}}
                },
                All = true
            });

            var existingContainer = existingContainers.FirstOrDefault();

            if (existingContainer != null)
            {
                var containerId = existingContainer.ID;
                _logger.LogDebug($"Found existing container with ID {containerId}");

                _logger.LogDebug($"Stopping container with ID {containerId}");
                await dockerClient.Containers.StopContainerAsync(containerId,
                    new ContainerStopParameters());

                _logger.LogDebug($"Removing container with ID {containerId}");
                await dockerClient.Containers.RemoveContainerAsync(containerId,
                    new ContainerRemoveParameters
                    {
                        RemoveVolumes = true
                    });
            }

            _logger.LogDebug($"Pulling image {_options.DockerPostgresImageName}:{_options.DockerPostgresImageTag}");
            await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = _options.DockerPostgresImageName,
                Tag = _options.DockerPostgresImageTag
            }, null, new ProgressMessageToILogger(_logger));

            _logger.LogDebug($"Creating new container with name {_options.DockerContainerName}");
            var createContainerParameters = new CreateContainerParameters
            {
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "5432", default
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "5432",
                            new List<PortBinding>
                                { new PortBinding { HostPort = _options.DockerPostgresHostPort.ToString() } }
                        }
                    },
                    PublishAllPorts = true
                },
                Env = new List<string>
                {
                    $"POSTGRES_USER={_options.DockerPostgresUser}",
                    $"POSTGRES_PASSWORD={_options.DockerPostgresPassword}"
                },
                Name = _options.DockerContainerName,
                Image = $"{_options.DockerPostgresImageName}:{_options.DockerPostgresImageTag}"
            };

            var createResponse = await dockerClient.Containers.CreateContainerAsync(createContainerParameters);

            _dockerContainerId = createResponse.ID;
            _logger.LogDebug($"Created new container with ID {_dockerContainerId}");

            _logger.LogDebug($"Starting container with ID {_dockerContainerId}");
            await dockerClient.Containers.StartContainerAsync(_dockerContainerId, new ContainerStartParameters());
        }

        private async Task WaitForServerToStartAsync()
        {
            var connectionString = GetConnectionString();
            _logger.LogDebug($"Creating connection with connection string: {connectionString}");
            await using var connection = new NpgsqlConnection(connectionString);

            var initialWait = _options.InitialWaitForPostgresStartupInSeconds;
            _logger.LogDebug($"Will now wait {initialWait} to give docker server time to start");
            Thread.Sleep(TimeSpan.FromSeconds(initialWait));

            var few = _options.SecondsToWaitBetweenPostgresConnectionRetries;
            var maxAttempts = _options.NumberOfTimesToAttemptConnectingToPostgres;
            var attemptCount = 0;
            var connectionOpened = false;
            Exception lastException = null;

            while (maxAttempts >= ++attemptCount)
            {
                lastException = null;
                try
                {
                    _logger.LogDebug("Attempting to open connection to database");
                    await connection.OpenAsync();
                    connectionOpened = true;
                    break;
                }
                catch (Exception e)
                {
                    lastException = e;
                    _logger.LogDebug($"Will now wait {few} more seconds because: {e.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(few));
                }
            }

            if (!connectionOpened)
            {
                throw new Exception($"Failed to open connection after {maxAttempts} attempts", lastException);
            }

            _logger.LogDebug("Connection opened successfully");
        }

        private async Task StopDockerInstanceAsync()
        {
            if (_dockerContainerId != null)
            {
                using var dockerClient = new DockerClientConfiguration()
                    .CreateClient();

                _logger.LogDebug($"Stopping container with ID {_dockerContainerId}");
                await dockerClient.Containers.StopContainerAsync(_dockerContainerId,
                    new ContainerStopParameters());
            }
        }

        private async Task RemoveDockerContainerAsync()
        {
            if (_dockerContainerId != null)
            {
                using var dockerClient = new DockerClientConfiguration()
                    .CreateClient();

                _logger.LogDebug($"Removing container with ID {_dockerContainerId}");
                await dockerClient.Containers.RemoveContainerAsync(_dockerContainerId,
                    new ContainerRemoveParameters
                    {
                        RemoveVolumes = true
                    });
            }
        }

    }
}
