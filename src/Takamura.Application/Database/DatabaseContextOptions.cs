using Microsoft.EntityFrameworkCore;

namespace Takamura.Application.Database;

/// <summary>SQL Server setup shared by the API (DI) and <c>dotnet ef</c> (<see cref="DatabaseContextFactory"/>).</summary>
public static class DatabaseContextOptions
{
    private const int MaxRetryCount = 3;

    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(5);

    public static DbContextOptionsBuilder UseTakamuraSqlServer(this DbContextOptionsBuilder options, string connectionString)
        => options.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.EnableRetryOnFailure(MaxRetryCount, MaxRetryDelay, errorNumbersToAdd: null));
}
