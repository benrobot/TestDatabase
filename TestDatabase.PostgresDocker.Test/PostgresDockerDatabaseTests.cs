using System;
using System.Threading.Tasks;
using Docker.DotNet;
using FluentAssertions;
using Npgsql;
using TestDatabase.PostgresDocker.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace TestDatabase.PostgresDocker.Test
{
    public class PostgresDockerDatabaseTests
    {
        private readonly TestLoggerForPostgresDockerDatabase _logger;
        private readonly DockerClient _dockerClient;

        public PostgresDockerDatabaseTests(ITestOutputHelper outputHelper)
        {
            _logger = new TestLoggerForPostgresDockerDatabase(outputHelper);
            _dockerClient = new DockerClientConfiguration().CreateClient();
        }

        [Fact]
        public async Task ContainerLifecyle()
        {
            const string containerName = "ContainerLifecylePostgres";

            using (var testDatabaseThatDoesntStop = new PostgresDockerDatabase(
                new PostgresDockerDatabaseOptions(stopDockerInstanceOnDispose: false, dockerPostgresHostPort: 5433,
                    dockerContainerName: containerName), _logger))
            {
                await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);

                await using var connection = new NpgsqlConnection(testDatabaseThatDoesntStop.GetConnectionString());
                await connection.OpenAsync();
                await connection.ShouldReportVersionAsync("PostgreSQL 14.2");
            }

            var firstContainersId = await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);

            using (var testDatabaseThatDoesntRemove = new PostgresDockerDatabase(
                new PostgresDockerDatabaseOptions(removeDockerContainerOnDispose: false, dockerPostgresHostPort: 5434,
                    dockerContainerName: containerName), _logger))
            {
                var secondContainersId = await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);
                firstContainersId.Should().NotBe(secondContainersId, "because a new container should be created each time");

                await using var connection = new NpgsqlConnection(testDatabaseThatDoesntRemove.GetConnectionString());
                await connection.OpenAsync();
                await connection.ShouldReportVersionAsync("PostgreSQL 14.2");
            }

            // The assertion for a stopped container seems to run faster than the docker engine can update its internal status. So adding a delay here.
            await Task.Delay(TimeSpan.FromSeconds(1));

            await _dockerClient.ShouldReportStoppedContainerNamedAsync(containerName);

            using (var testDatabaseThatRemoves = new PostgresDockerDatabase(
                new PostgresDockerDatabaseOptions(dockerPostgresHostPort: 5435,
                    dockerContainerName: containerName), _logger))
            {
                await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);

                await using var connection = new NpgsqlConnection(testDatabaseThatRemoves.GetConnectionString());
                await connection.OpenAsync();
                await connection.ShouldReportVersionAsync("PostgreSQL 14.2");
            }

            await _dockerClient.ShouldReportNoContainerNamedAsync(containerName);
        }
    }
}
