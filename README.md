# Test Database for SQL Server using Docker ![Logo created using LogoMakr.com](logo_by_LogoMakr.png?raw=true)

A real SQL Server database running on docker for your unit tests.

Published at https://www.nuget.org/packages/TestDatabase.SqlServerDocker/

See examples at https://github.com/benrobot/TestDatabase.Sample

#### Alternatives

[DotNet.Testcontainers](https://www.nuget.org/packages/DotNet.Testcontainers/) - I found out about **DotNet.Testcontainers** after creating this package but I have yet to try it out.

## Usage (SQL Server)

#### Minimal example
```csharp
// Constructor starts a docker container and waits for SQL Server to be ready
using (var testDatabase = new SqlServerDockerDatabase(new SqlServerDockerDatabaseOptions()))
{
    // Returns connection string to newly created database server
    var connectionString = testDatabase.GetConnectionString(); 
    
    // Do stuff with a real SQL Server database
}
// Dispose automatically stops and removes docker container
```

#### Use a different port
Perhaps you don't want it to use port 1433 because you have another instance of SQL Server running locally. In this example we set the port to 1337 instead by setting the `dockerSqlServerHostPort` parameter.

```csharp
var options = new SqlServerDockerDatabaseOptions(dockerSqlServerHostPort: 1337);
using (var testDatabase = new SqlServerDockerDatabase(options))
{ 
    var connectionString = testDatabase.GetConnectionString();
    // Do stuff with a real SQL Server database
}
```

#### Leave it running after the test completes
Sometimes you're dealing with a particulary pesky problem and you want to connect to the same database against which your tests just ran. This example shows how to leave the the SQL Server running after the test has completed using the `stopDockerInstanceOnDispose` parameter.

```csharp
var options = new SqlServerDockerDatabaseOptions(stopDockerInstanceOnDispose: false);
using (var testDatabase = new SqlServerDockerDatabase(options))
{ 
    var connectionString = testDatabase.GetConnectionString();
    // Do stuff with a real SQL Server database
}
```

#### All options on the table

Sample code showing all options changed. The settings shown in the code below are **NOT** the defaults. For defaults see [Parameter Table](#Parameter-Table-SqlServer)
```csharp
// These are NOT the default values
var options = new SqlServerDockerDatabaseOptions(
    dockerContainerName: "MyTestDatabase",
    dockerSqlServerPassword: "myEvenStronger(!)Password",
    dockerSqlServerHostPort: 1337,
    dockerSqlServerImageName: "mcr.microsoft.com/mssql/server",
    dockerSqlServerImageTag: "2022-latest",
    stopDockerInstanceOnDispose: false,
    removeDockerContainerOnDispose: false,
    initialWaitForSqlServerStartupInSeconds: 20,
    secondsToWaitBetweenSqlServerConnectionRetries: 3,
    numberOfTimesToAttemptConnectingToSqlServer: 15
);
using (var testDatabase = new SqlServerDockerDatabase(options))
{ 
    var connectionString = testDatabase.GetConnectionString();
    // Do stuff with a real SQL Server database
}
```

##### Parameter Table SqlServer
| Parameter                                      | Default                                     | Description |
| ---------------------------------------------- | ------------------------------------------- | ----------- |
| dockerContainerName                            | *First 30 characters of your assembly name* | The value that appears in the NAMES column if you execute `docker ps` from the command line |
| dockerSqlServerPassword                        | `yourStrong(!)Password`                     | This will be the `sa` password. Don't make it too simple because otherwise the SQL Server database will reject it because it does not meet certain password complexity requirements. See [Microsoft SQL Server Password Policy](https://docs.microsoft.com/en-us/sql/relational-databases/security/password-policy?view=sql-server-ver15). This is the same password that is included in the connection string returned by the `GetConnectionString()` method. |
| dockerSqlServerHostPort                        | 1433                                        | The port on which Docker will expose the server. Inside the container SQL Server is still running on port 1433 but that is abstracted away by the magic of Docker. |
| dockerSqlServerImageName                       | `mcr.microsoft.com/mssql/server`            | The Docker image name used to `docker pull ...` |
| dockerSqlServerImageTag                        | `2022-CU14-ubuntu-22.04`                    | The Docker image tag used to `docker pull ...`  |
| stopDockerInstanceOnDispose                    | true                                        | If both `stopDockerInstanceOnDispose` and `removeDockerContainerOnDispose` are unspecified then the default is `true`. However, if `removeDockerContainerOnDispose` is explicitly specified as `true` then `stopDockerInstanceOnDispose` is ignored (even if it was specified to be `false`). Similarly, if `stopDockerInstanceOnDispose` is specified as `false` and `removeDockerContainerOnDispose` is unspecified then `removeDockerContainerOnDispose` is effectively `false`. Regardless of these settings the **SqlServerDockerDatabase** constructor always stops and removes any previous containers with the same name before creating a brand new container. |
| removeDockerContainerOnDispose                 | true                                        | See caveat under the description for `stopDockerInstanceOnDispose` |
| initialWaitForSqlServerStartupInSeconds        | 10                                          | The number of seconds to wait before the **SqlServerDockerDatabase** constructor begins attempting to connect to the SQL Server to verify it is ready before returning. |
| secondsToWaitBetweenSqlServerConnectionRetries | 5                                           | The number of seconds the **SqlServerDockerDatabase** constructor waits between attempting to connect to SQL Server to verify it is ready before returning |
| numberOfTimesToAttemptConnectingToSqlServer    | 10                                          | The number of times the **SqlServerDockerDatabase** constructor tries to connect to SQL Server to verify it is ready. If the tries are exhausted then the **SqlServerDockerDatabase** constructor throws an exception. The last exception thrown by `await connection.OpenAsync()` is included as an inner exception |

#### More Examples
See more examples at https://github.com/benrobot/TestDatabase.Sample

## Usage (Postgres)

#### Minimal example
```csharp
// Constructor starts a docker container and waits for SQL Server to be ready
using (var testDatabase = new PostgresDockerDatabase(new PostgresDockerDatabaseOptions()))
{
    // Returns connection string to newly created database server
    var connectionString = testDatabase.GetConnectionString(); 
    
    // Do stuff with a real SQL Server database
}
// Dispose automatically stops and removes docker container
```

#### Use a different port
Perhaps you don't want it to use port 1433 because you have another instance of SQL Server running locally. In this example we set the port to 1337 instead by setting the `dockerPostgresHostPort` parameter.

```csharp
var options = new PostgresDockerDatabaseOptions(dockerPostgresHostPort: 1337);
using (var testDatabase = new PostgresDockerDatabase(options))
{ 
    var connectionString = testDatabase.GetConnectionString();
    // Do stuff with a real SQL Server database
}
```

#### Leave it running after the test completes
Sometimes you're dealing with a particulary pesky problem and you want to connect to the same database against which your tests just ran. This example shows how to leave the the SQL Server running after the test has completed using the `stopDockerInstanceOnDispose` parameter.

```csharp
var options = new PostgresDockerDatabaseOptions(stopDockerInstanceOnDispose: false);
using (var testDatabase = new PostgresDockerDatabase(options))
{ 
    var connectionString = testDatabase.GetConnectionString();
    // Do stuff with a real SQL Server database
}
```

#### All options on the table

Sample code showing all options changed. The settings shown in the code below are **NOT** the defaults. For defaults see [Parameter Table](#Parameter-Table-Postgres)
```csharp
// These are NOT the default values
var options = new PostgresDockerDatabaseOptions(
    dockerContainerName: "MyTestDatabase",
    dockerPostgresUser: "myuser",
    dockerPostgresPassword: "myevenstrongersecretpassword",
    dockerPostgresHostPort: 1337,
    dockerPostgresImageName: "postgres",
    dockerPostgresImageTag: "16.4-bullseye",
    stopDockerInstanceOnDispose: false,
    removeDockerContainerOnDispose: false,
    initialWaitForPostgresStartupInSeconds: 20,
    secondsToWaitBetweenPostgresConnectionRetries: 3,
    numberOfTimesToAttemptConnectingToPostgres: 15
);
using (var testDatabase = new PostgresDockerDatabase(options))
{ 
    var connectionString = testDatabase.GetConnectionString();
    // Do stuff with a real SQL Server database
}
```

##### Parameter Table Postgres
| Parameter                                      | Default                                     | Description |
| ---------------------------------------------- | ------------------------------------------- | ----------- |
| dockerContainerName                            | *First 30 characters of your assembly name* | The value that appears in the NAMES column if you execute `docker ps` from the command line |
| dockerPostgresUser                             | `postgres`                                  | This will be the username used to connect to the database. |
| dockerPostgresPassword                         | `mysecretpassword`                          | This will be the password used to connect to the database. |
| dockerPostgresHostPort                         | 5432                                        | The port on which Docker will expose the server. Inside the container Postgres is still running on port 5432 but that is abstracted away by the magic of Docker. |
| dockerPostgresImageName                        | `postgres`                                  | The Docker image name used to `docker pull ...` |
| dockerPostgresImageTag                         | `16.4-alpine`                               | The Docker image tag used to `docker pull ...`  |
| stopDockerInstanceOnDispose                    | true                                        | If both `stopDockerInstanceOnDispose` and `removeDockerContainerOnDispose` are unspecified then the default is `true`. However, if `removeDockerContainerOnDispose` is explicitly specified as `true` then `stopDockerInstanceOnDispose` is ignored (even if it was specified to be `false`). Similarly, if `stopDockerInstanceOnDispose` is specified as `false` and `removeDockerContainerOnDispose` is unspecified then `removeDockerContainerOnDispose` is effectively `false`. Regardless of these settings the **PostgresDockerDatabase** constructor always stops and removes any previous containers with the same name before creating a brand new container. |
| removeDockerContainerOnDispose                 | true                                        | See caveat under the description for `stopDockerInstanceOnDispose` |
| initialWaitForPostgresStartupInSeconds         | 10                                          | The number of seconds to wait before the **PostgresDockerDatabase** constructor begins attempting to connect to the SQL Server to verify it is ready before returning. |
| secondsToWaitBetweenPostgresConnectionRetries  | 5                                           | The number of seconds the **PostgresDockerDatabase** constructor waits between attempting to connect to SQL Server to verify it is ready before returning |
| numberOfTimesToAttemptConnectingToPostgres     | 10                                          | The number of times the **PostgresDockerDatabase** constructor tries to connect to SQL Server to verify it is ready. If the tries are exhausted then the **PostgresDockerDatabase** constructor throws an exception. The last exception thrown by `await connection.OpenAsync()` is included as an inner exception |

#### More Examples
See more examples at https://github.com/benrobot/TestDatabase.Sample