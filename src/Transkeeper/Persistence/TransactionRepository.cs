using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Transkeeper.Abstractions;
using Transkeeper.Models;

namespace Transkeeper.Persistence;

public class TransactionRepository : IRepository<Transaction>, IDisposable
{
    private readonly SqliteConnection _connection;

    public TransactionRepository(IConfiguration configuration)
    {
        var cs = configuration.GetSection("SQLite:ConnectionString").Value;

        if (string.IsNullOrEmpty(cs))
        {
            throw new ArgumentException("Connection string must not be null or empty.", nameof(configuration));
        }

        try
        {
            _connection = new SqliteConnection(cs);
        }
        catch (ArgumentException e)
        {
            throw new ArgumentException("Connection string is in invalid format.", nameof(configuration), e);
        }
    }

    public Transaction? GetById(int id)
    {
        try
        {
            return _connection.Query<Transaction>(@"SELECT * FROM [Transaction]
                                                     WHERE Id = @Id;", new { Id = id })
                                                     .FirstOrDefault();
        }
        catch (SqliteException e)
        {
            throw new InvalidOperationException("Failed to read from data source.", e);
        }
    }

    public bool TrySave(Transaction item)
    {
        try
        {
            var rows = _connection.Execute(@"INSERT INTO [Transaction]
                                                 (Id, Amount, TransactionDate)
                                                 VALUES (@Id, @Amount, @TransactionDate);", item);

            return true;
        }
        catch (SqliteException e) when(e.ErrorCode == -2147467259)
        {
            return false;
        }
        catch (SqliteException e)
        {
            throw new InvalidOperationException("Failed to write to data source.", e);
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}