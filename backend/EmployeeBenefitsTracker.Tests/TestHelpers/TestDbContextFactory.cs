using EmployeeBenefitsTracker.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeBenefitsTracker.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static (AppDbContext context, SqliteConnection connection) Create()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }
}
