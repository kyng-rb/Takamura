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

    public async Task<Result<RetrieveMonthlySubCategoryBalanceOutput>> Handle(RetrieveMonthlySubCategoryBalanceInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Invalid budget id");

        if (input.Year <= 0)
            return Result.Fail("Invalid year");

        if (input.Month.HasValue && (input.Month.Value < 1 || input.Month.Value > 12))
            return Result.Fail("Invalid month");

        var exists = await _context.Budgets
            .AnyAsync(x => x.Id == input.BudgetId)
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
            .ToListAsync()
            .ConfigureAwait(false);

        return Result.Ok(RetrieveMonthlySubCategoryBalanceOutput.FromEntities(records));
    }
}
