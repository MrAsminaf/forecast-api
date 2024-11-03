using System.Data.Common;
using ForecastAPI.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ForecastAPI.UnitTests;

public abstract class UnitTestsBase : IDisposable
{
    protected readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationContext> _contextOptions;

    public UnitTestsBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(_connection)
            .Options;

        using (var context = new ApplicationContext(_contextOptions))
        {
            context.Database.EnsureCreated();
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    protected ApplicationContext CreateContext() => new ApplicationContext(_contextOptions);
}
