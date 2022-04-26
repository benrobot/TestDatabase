using FluentAssertions;
using Xunit;

namespace TestDatabase.PostgresDocker.Test
{
    public class PostgresDockerDatabaseOptionsTests
    {
        [Fact]
        public void Defaults()
        {
            // Act
            var options = new PostgresDockerDatabaseOptions();

            // Assert
            options.DockerContainerName.Should().Be("TestDatabase_PostgresDocker_Te");
            options.DockerPostgresUser.Should().Be("postgres");
            options.DockerPostgresPassword.Should().Be("mysecretpassword");
            options.DockerPostgresHostPort.Should().Be(5432);
            options.DockerPostgresImageName.Should().Be("postgres");
            options.DockerPostgresImageTag.Should().Be("14.2-alpine");
            options.StopDockerInstanceOnDispose.Should().BeTrue();
            options.RemoveDockerContainerOnDispose.Should().BeTrue();
        }

        [Fact]
        public void Overrides()
        {
            // Act
            var options = new PostgresDockerDatabaseOptions(
                "container name", 
                "username",
                "password", 
                1234, 
                "image name", 
                "image tag", 
                false, 
                false);

            // Assert
            options.DockerContainerName.Should().Be("container name");
            options.DockerPostgresUser.Should().Be("username");
            options.DockerPostgresPassword.Should().Be("password");
            options.DockerPostgresHostPort.Should().Be(1234);
            options.DockerPostgresImageName.Should().Be("image name");
            options.DockerPostgresImageTag.Should().Be("image tag");
            options.StopDockerInstanceOnDispose.Should().BeFalse();
            options.RemoveDockerContainerOnDispose.Should().BeFalse();
        }

        [Fact]
        public void Setting_StopDockerInstanceOnDispose_to_false_forces_RemoveDockerContainerOnDispose_to_be_false()
        {
            // Act
            var options = new PostgresDockerDatabaseOptions(stopDockerInstanceOnDispose: false);

            // Assert
            options.StopDockerInstanceOnDispose.Should().BeFalse();
            options.RemoveDockerContainerOnDispose.Should().BeFalse();
        }

        [Fact]
        public void Setting_RemoveDockerContainerOnDispose_to_true_forces_RemoveDockerContainerOnDispose_to_be_true()
        {
            // Act
            var options = new PostgresDockerDatabaseOptions(stopDockerInstanceOnDispose: false, removeDockerContainerOnDispose: true);

            // Assert
            options.StopDockerInstanceOnDispose.Should().BeTrue();
            options.RemoveDockerContainerOnDispose.Should().BeTrue();
        }
    }
}
