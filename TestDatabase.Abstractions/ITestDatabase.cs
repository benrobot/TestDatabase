using System;
using System.Data;

namespace TestDatabase.Abstractions
{
    /// <summary>
    /// Starts a docker container running a database and blocks until the database is ready to accept connections
    /// </summary>
    public interface ITestDatabase : IDisposable
    {
        /// <summary>
        /// Returns connection string for the database server
        /// </summary>
        /// <returns>Connection string</returns>
        string GetConnectionString();

        /// <summary>
        /// Returns connection string with database name for the database server
        /// </summary>
        /// <param name="databaseName">Database name to include in the connection string</param>
        /// <returns>Connection string</returns>
        string GetConnectionString(string databaseName);

        /// <summary>
        /// Returns a new database connection to the database server. Connection is already open.
        /// </summary>
        /// <returns>New open connection</returns>
        IDbConnection GetNewDbConnection();

        /// <summary>
        /// Returns a new database connection to the database server and specified database name. Connection is already open.
        /// </summary>
        /// <param name="databaseName">Database name to connect to</param>
        /// <returns>New open connection</returns>
        IDbConnection GetNewDbConnection(string databaseName);
    }
}
