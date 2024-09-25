using FluentAssertions;
using Xunit;

namespace TestDatabase.SqlServerDocker.Test
{
    public class SqlServerDockerDatabaseOptionsTests
    {
        [Fact]
        public void Defaults()
        {
            // Act
            var options = new SqlServerDockerDatabaseOptions();

            // Assert
            options.DockerContainerName.Should().Be("TestDatabase_SqlServerDocker_T");
            options.DockerSqlServerPassword.Should().Be("yourStrong(!)Password");
            options.DockerSqlServerHostPort.Should().Be(1433);
            options.DockerSqlServerImageName.Should().Be("mcr.microsoft.com/mssql/server");
            options.DockerSqlServerImageTag.Should().Be("2022-CU14-ubuntu-22.04");
            options.StopDockerInstanceOnDispose.Should().BeTrue();
            options.RemoveDockerContainerOnDispose.Should().BeTrue();
        }

        [Fact]
        public void Overrides()
        {
            // Act
            var options = new SqlServerDockerDatabaseOptions(
                "container name", 
                "password", 
                1234, 
                "image name", 
                "image tag", 
                false, 
                false);

            // Assert
            options.DockerContainerName.Should().Be("container name");
            options.DockerSqlServerPassword.Should().Be("password");
            options.DockerSqlServerHostPort.Should().Be(1234);
            options.DockerSqlServerImageName.Should().Be("image name");
            options.DockerSqlServerImageTag.Should().Be("image tag");
            options.StopDockerInstanceOnDispose.Should().BeFalse();
            options.RemoveDockerContainerOnDispose.Should().BeFalse();
        }

        [Fact]
        public void Setting_StopDockerInstanceOnDispose_to_false_forces_RemoveDockerContainerOnDispose_to_be_false()
        {
            // Act
            var options = new SqlServerDockerDatabaseOptions(stopDockerInstanceOnDispose: false);

            // Assert
            options.StopDockerInstanceOnDispose.Should().BeFalse();
            options.RemoveDockerContainerOnDispose.Should().BeFalse();
        }

        [Fact]
        public void Setting_RemoveDockerContainerOnDispose_to_true_forces_RemoveDockerContainerOnDispose_to_be_true()
        {
            // Act
            var options = new SqlServerDockerDatabaseOptions(stopDockerInstanceOnDispose: false, removeDockerContainerOnDispose: true);

            // Assert
            options.StopDockerInstanceOnDispose.Should().BeTrue();
            options.RemoveDockerContainerOnDispose.Should().BeTrue();
        }
    }
}
