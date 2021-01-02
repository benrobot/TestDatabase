using System;
using System.Data;

namespace TestDatabase.Abstractions
{
    public interface ITestDatabase : IDisposable
    {
        string GetConnectionString();
        string GetConnectionString(string databaseName);
        IDbConnection GetNewDbConnection();
        IDbConnection GetNewDbConnection(string databaseName);
    }
}
