﻿using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;
using TestDatabase.Abstractions;

namespace TestDatabase.SqlServerDocker
{
    public class SqlServerDockerDatabase : ITestDatabase
    {
        private readonly ILogger<SqlServerDockerDatabase> _logger;
        private readonly SqlServerDockerDatabaseOptions _options;

        private string _dockerContainerId;

        public SqlServerDockerDatabase(SqlServerDockerDatabaseOptions options, ILogger<SqlServerDockerDatabase> logger = null)
        {
            _logger = logger ?? new SqlServerDockerDatabaseSystemDebugLogger();
            _options = options;
            StartDockerInstanceAsync().Wait();
            WaitForServerToStartAsync().Wait();
        }

        public string GetConnectionString() => $"Data Source=tcp:localhost,{_options.DockerSqlServerHostPort};User Id=sa;Password={_options.DockerSqlServerPassword};";

        public string GetConnectionString(string databaseName) =>
            $"{GetConnectionString()};Initial Catalog={databaseName}";

        public IDbConnection GetNewDbConnection()
        {
            var conn = new SqlConnection(GetConnectionString());
            conn.Open();
            return conn;
        }

        public IDbConnection GetNewDbConnection(string databaseName)
        {
            var conn = new SqlConnection($"{GetConnectionString()};Initial Catalog={databaseName}");
            conn.Open();
            return conn;
        }

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

            _logger.LogDebug($"Pulling image {_options.DockerSqlServerImageName}:{_options.DockerSqlServerImageTag}");
            await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = _options.DockerSqlServerImageName,
                Tag = _options.DockerSqlServerImageTag
            }, null, new ProgressMessageToILogger(_logger));

            _logger.LogDebug($"Creating new container with name {_options.DockerContainerName}");
            var createResponse = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "1433", default
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {"1433", new List<PortBinding> {new PortBinding {HostPort = _options.DockerSqlServerHostPort.ToString() } }}
                    },
                    PublishAllPorts = true
                },
                Env = new List<string> { "ACCEPT_EULA=Y", $"SA_PASSWORD={_options.DockerSqlServerPassword}" },
                Name = _options.DockerContainerName,
                Image = $"{_options.DockerSqlServerImageName}:{_options.DockerSqlServerImageTag}"
            });

            _dockerContainerId = createResponse.ID;
            _logger.LogDebug($"Created new container with ID {_dockerContainerId}");

            _logger.LogDebug($"Starting container with ID {_dockerContainerId}");
            await dockerClient.Containers.StartContainerAsync(_dockerContainerId, new ContainerStartParameters());
        }

        private async Task WaitForServerToStartAsync()
        {
            using var connection = new SqlConnection(GetConnectionString());

            var initialWait = 10;
            _logger.LogDebug($"Will now wait {initialWait} to give docker server time to start");
            Thread.Sleep(TimeSpan.FromSeconds(initialWait));

            var few = 5;
            var maxAttempts = 10;
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
