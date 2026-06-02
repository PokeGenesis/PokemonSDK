namespace SDK.Data.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public sealed class SqliteTestFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteTestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        using var ctx = CreateContext();
        ctx.Database.EnsureCreated();
    }

    public PokemonDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new PokemonDbContext(options);
    }

    public void Dispose() => _connection.Dispose();
}
