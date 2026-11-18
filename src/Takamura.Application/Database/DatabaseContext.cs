using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Base.Enums;
using Takamura.Application.Database.Entities.Balance;
using Takamura.Application.Database.Entities.Bill;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.Category;
using Takamura.Application.Database.Entities.PeriodAllocation;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        ApplySoftDeleteFilter(modelBuilder);

        modelBuilder.Entity<MonthlySubCategoryBalanceEntity>()
            .HasNoKey();

        var monthlySubCategoryBalanceMethod = typeof(DatabaseContext)
            .GetMethod(nameof(GetMonthlySubCategoryBalance), [typeof(int), typeof(int), typeof(int?)])!;

        modelBuilder.HasDbFunction(monthlySubCategoryBalanceMethod)
            .HasName("monthly_sub_category_balance")
            .HasSchema("dbo");

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Hides rows with <see cref="Status.Deleted"/> from every query (including <c>AnyAsync</c> existence checks).
    /// Use <c>IgnoreQueryFilters()</c> when a query really needs deleted rows.
    /// </summary>
    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        var softDeletableTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(EntityBase).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in softDeletableTypes)
        {
            var entity = Expression.Parameter(entityType.ClrType, "entity");
            var notDeleted = Expression.NotEqual(
                Expression.Property(entity, nameof(EntityBase.Status)),
                Expression.Constant(Status.Deleted));

            entityType.SetQueryFilter(Expression.Lambda(notDeleted, entity));
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HaveColumnType($"decimal(18,{FieldLimits.MoneyDecimals})");

        configurationBuilder.Properties<string>()
            .HaveColumnType($"varchar({FieldLimits.TextMaxLength})");

        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>()
            .HaveColumnType($"varchar({FieldLimits.EnumMaxLength})");

        configurationBuilder.Properties<DateTime>()
            .HaveColumnType("datetime2")
            .HaveConversion<UtcDateTimeConverter>();

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
