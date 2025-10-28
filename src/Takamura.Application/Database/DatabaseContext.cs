using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Takamura.Application.Database.Entities.Bill;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.Category;
using Takamura.Application.Database.Entities.PeriodAllocation;
using Takamura.Application.Database.Entities.SubCategory;
using Takamura.Application.Database.Entities.Summary;
using Takamura.Application.Features.Budget.Summary;

namespace Takamura.Application.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        modelBuilder
        .HasDbFunction(typeof(DatabaseContext).GetMethod(nameof(GetMonthlyCategoryBalance), new[] { typeof(int), typeof(int), typeof(int?) }))
        .HasName("monthly_category_balance");

        modelBuilder
        .HasDbFunction(typeof(DatabaseContext).GetMethod(nameof(GetMonthlySubCategoryBalance), new[] { typeof(int), typeof(int), typeof(int?) }))
        .HasName("monthly_sub_category_balance");

        modelBuilder
        .HasDbFunction(typeof(DatabaseContext).GetMethod(nameof(GetYearToDateBalance), new[] { typeof(int) }))
        .HasName("year_to_date_balance");

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

    public DbSet<PeriodAllocationEntity> PeriodAllocations => Set<PeriodAllocationEntity>();

    public DbSet<BillEntity> Bills => Set<BillEntity>();

    public IQueryable<MonthlyCategoryBalance> GetMonthlyCategoryBalance(int budgetId, int year, int? month)
        => FromExpression(() => GetMonthlyCategoryBalance(budgetId, year, month));

    public IQueryable<MonthlySubCategoryBalance> GetMonthlySubCategoryBalance(int budgetId, int year, int? month)
        => FromExpression(() => GetMonthlySubCategoryBalance(budgetId, year, month));

    public IQueryable<YearToDateBalance> GetYearToDateBalance(int budgetId)
        => FromExpression(() => GetYearToDateBalance(budgetId));
}