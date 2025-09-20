using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace Takamura.Application.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    private const string ConnectionStringEnv = "TakamuraConnectionString";
    
    public DatabaseContext CreateDbContext(string[] args)
    {
        var connectionSting = Environment.GetEnvironmentVariable(ConnectionStringEnv);
        
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

        optionsBuilder.UseSqlServer(connectionSting,
            sqlOptions => sqlOptions
                .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), default));

        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        optionsBuilder.EnableDetailedErrors();

        return new DatabaseContext(optionsBuilder.Options);
    }
}