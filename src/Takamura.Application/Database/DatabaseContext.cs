using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Takamura.Application.Database.Entities.Bill;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.Category;
using Takamura.Application.Database.Entities.MonthlyBudget;
using Takamura.Application.Database.Entities.SubCategory;
using Takamura.Application.Database.Entities.SubCategoryBudget;

namespace Takamura.Application.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) 
        :base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HaveColumnType("decimal(18,2)");
        
        configurationBuilder.Properties<string>()
            .HaveColumnType("varchar(100)");
        
        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>()
            .HaveColumnType("varchar(20)");

        configurationBuilder.Properties<DateTime>()
            .HaveColumnType("datetime");
        
        base.ConfigureConventions(configurationBuilder);
    }

    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    public DbSet<SubCategoryEntity> SubCategories => Set<SubCategoryEntity>();
    
    public DbSet<BudgetEntity> Budgets => Set<BudgetEntity>();
    
    public DbSet<SubCategoryBudgetEntity> SubCategoryBudgets => Set<SubCategoryBudgetEntity>();
    
    public DbSet<MonthlyBudgetEntity> MonthlyBudgets => Set<MonthlyBudgetEntity>();
    
    public DbSet<BillEntity> Bills => Set<BillEntity>();
}