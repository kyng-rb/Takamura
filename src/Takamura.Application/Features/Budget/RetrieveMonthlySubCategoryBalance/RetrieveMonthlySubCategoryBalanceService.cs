using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Balance;

namespace Takamura.Application.Features.Budget.RetrieveMonthlySubCategoryBalance;

public record RetrieveMonthlySubCategoryBalanceInput(int BudgetId, int Year, int? Month);

public record MonthlySubCategoryBalanceOutput(
    string Budget,
    decimal PeriodBudget,
    decimal AvailableAmount,
    int Month,
    int Year,
    string Category,
    string SubCategory)
{
    public static MonthlySubCategoryBalanceOutput FromEntity(MonthlySubCategoryBalanceEntity entity)
        => new(
            entity.Budget,
            entity.PeriodBudget,
            entity.AvailableAmount,
            entity.Month,
            entity.Year,
            entity.Category,
            entity.SubCategory);
}

public record RetrieveMonthlySubCategoryBalanceOutput(IEnumerable<MonthlySubCategoryBalanceOutput> Balances)
{
    public static RetrieveMonthlySubCategoryBalanceOutput FromEntities(IEnumerable<MonthlySubCategoryBalanceEntity> entities)
        => new(entities.Select(MonthlySubCategoryBalanceOutput.FromEntity));
}

public class RetrieveMonthlySubCategoryBalanceService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrieveMonthlySubCategoryBalanceOutput>> Handle(RetrieveMonthlySubCategoryBalanceInput input, CancellationToken cancellationToken = default)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Budget id is required");

        if (input.Year is < FieldLimits.MinYear or > FieldLimits.MaxYear)
            return Result.Fail($"Year must be between {FieldLimits.MinYear} and {FieldLimits.MaxYear}");

        if (input.Month.HasValue && (input.Month.Value < 1 || input.Month.Value > 12))
            return Result.Fail("Month must be between 1 and 12");

        var exists = await _context.Budgets
            .AnyAsync(x => x.Id == input.BudgetId, cancellationToken)
            .ConfigureAwait(false);
        if (!exists)
            return Result.Fail("Budget does not exist");

        var records = await _context
            .GetMonthlySubCategoryBalance(input.BudgetId, input.Year, input.Month)
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ThenBy(x => x.Category)
            .ThenBy(x => x.SubCategory)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result.Ok(RetrieveMonthlySubCategoryBalanceOutput.FromEntities(records));
    }
}
