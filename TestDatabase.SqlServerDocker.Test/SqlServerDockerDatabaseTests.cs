using System.Threading.Tasks;
using Docker.DotNet;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using TestDatabase.SqlServerDocker.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace TestDatabase.SqlServerDocker.Test
{
    public class SqlServerDockerDatabaseTests
    {
        private readonly TestLoggerForSqlServerDockerDatabase _logger;
        private readonly DockerClient _dockerClient;

        public SqlServerDockerDatabaseTests(ITestOutputHelper outputHelper)
        {
            _logger = new TestLoggerForSqlServerDockerDatabase(outputHelper);
            _dockerClient = new DockerClientConfiguration().CreateClient();
        }

        [Fact]
        public async Task ContainerLifecyle()
        {
            const string containerName = "ContainerLifecyleSqlServer";

            using (var testDatabaseThatDoesntStop = new SqlServerDockerDatabase(
                new SqlServerDockerDatabaseOptions(stopDockerInstanceOnDispose: false, dockerSqlServerHostPort: 1457,
                    dockerContainerName: containerName), _logger))
            {
                await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);

                await using var connection = new SqlConnection(testDatabaseThatDoesntStop.GetConnectionString());
                await connection.OpenAsync();
                await connection.ShouldReportVersionAsync("Microsoft SQL Server 2019");
            }

            var firstContainersId = await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);

            using (var testDatabaseThatDoesntRemove = new SqlServerDockerDatabase(
                new SqlServerDockerDatabaseOptions(removeDockerContainerOnDispose: false, dockerSqlServerHostPort: 1457,
                    dockerContainerName: containerName), _logger))
            {
                var secondContainersId = await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);
                firstContainersId.Should().NotBe(secondContainersId, "because a new container should be created each time");

                await using var connection = new SqlConnection(testDatabaseThatDoesntRemove.GetConnectionString());
                await connection.OpenAsync();
                await connection.ShouldReportVersionAsync("Microsoft SQL Server 2019");
            }

            await _dockerClient.ShouldReportStoppedContainerNamedAsync(containerName);

            using (var testDatabaseThatRemoves = new SqlServerDockerDatabase(
                new SqlServerDockerDatabaseOptions(dockerSqlServerHostPort: 1457,
                    dockerContainerName: containerName), _logger))
            {
                await _dockerClient.ShouldReportRunningContainerNamedAsync(containerName);

                await using var connection = new SqlConnection(testDatabaseThatRemoves.GetConnectionString());
                await connection.OpenAsync();
                await connection.ShouldReportVersionAsync("Microsoft SQL Server 2019");
            }

            await _dockerClient.ShouldReportNoContainerNamedAsync(containerName);
        }
    }
}
