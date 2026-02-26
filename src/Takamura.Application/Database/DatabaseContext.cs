using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database.Entities.Balance;
using Takamura.Application.Database.Entities.Bill;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.Category;
using Takamura.Application.Database.Entities.PeriodBudget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        modelBuilder.Entity<MonthlySubCategoryBalanceEntity>()
            .HasNoKey();

        var monthlySubCategoryBalanceMethod = typeof(DatabaseContext)
            .GetMethod(nameof(GetMonthlySubCategoryBalance), [typeof(int), typeof(int), typeof(int?)])!;

        modelBuilder.HasDbFunction(monthlySubCategoryBalanceMethod)
            .HasName("monthly_sub_category_balance")
            .HasSchema("dbo");

        base.OnModelCreating(modelBuilder);
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

    public DbSet<PeriodAllocationEntity> PeriodAllocations => Set<PeriodAllocationEntity>();

    public DbSet<BillEntity> Bills => Set<BillEntity>();

    public IQueryable<MonthlySubCategoryBalanceEntity> GetMonthlySubCategoryBalance(int budgetId, int year, int? month)
        => FromExpression(() => GetMonthlySubCategoryBalance(budgetId, year, month));
}
