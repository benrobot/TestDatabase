using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.Data.SqlClient;

namespace TestDatabase.SqlServerDocker.Test.Helpers
{
    public static class FluentAssertionExtensions
    {
        public static async Task<string> ShouldReportRunningContainerNamedAsync(this DockerClient client, string containerName)
        {
            var existingContainersWithMatchingName = await ExistingContainersWithMatchingName(client, containerName);
            existingContainersWithMatchingName.Should().HaveCount(1);
            var container = existingContainersWithMatchingName.First();
            container.Status.Should().StartWith("Up ");
            return container.ID;
        }

        public static async Task ShouldReportStoppedContainerNamedAsync(this DockerClient client, string containerName)
        {
            var existingContainersWithMatchingName = await ExistingContainersWithMatchingName(client, containerName);
            existingContainersWithMatchingName.Should().HaveCount(1);
            var container = existingContainersWithMatchingName.First();
            container.Status.Should().StartWith("Exited ");
        }

        public static async Task ShouldReportNoContainerNamedAsync(this DockerClient client, string containerName)
        {
            var existingContainersWithMatchingName = await ExistingContainersWithMatchingName(client, containerName);
            existingContainersWithMatchingName.Should().BeEmpty();
        }

        public static async Task ShouldReportVersionAsync(this SqlConnection connection, string version)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT @@VERSION";
            var sqlVersionObject = await command.ExecuteScalarAsync();
            sqlVersionObject.Should().BeAssignableTo<string>();
            var sqlVersion = (string)sqlVersionObject;
            sqlVersion.Should().StartWith(version);
        }

        private static async Task<IList<ContainerListResponse>> ExistingContainersWithMatchingName(DockerClient client, string containerName)
        {
            var existingContainersWithMatchingName = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"name", new Dictionary<string, bool> {{containerName, true}}}
                },
                All = true
            });
            return existingContainersWithMatchingName;
        }
    }
}
