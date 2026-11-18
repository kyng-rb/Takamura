using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace Takamura.Application.Database;

/// <summary>
/// Used only by <c>dotnet ef</c> (migrations add / database update). Reads the connection string from
/// the <c>TakamuraConnectionString</c> environment variable.
/// </summary>
public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    private const string ConnectionStringEnv = "TakamuraConnectionString";

    public DatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnv);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"Set the {ConnectionStringEnv} environment variable before running dotnet ef " +
                "(for the local container: set -a; . ./.env; set +a).");

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => sqlOptions
                .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), default));

        // No sensitive data logging: this factory can point at the production database,
        // and parameter values would end up in the terminal.
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning);

        return new DatabaseContext(optionsBuilder.Options);
    }
}
