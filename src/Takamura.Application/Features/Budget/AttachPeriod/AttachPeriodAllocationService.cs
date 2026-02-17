using FluentResults;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.PeriodBudget;

namespace Takamura.Application.Features.Budget.AttachPeriod;

public record AttachPeriodAllocationServiceInput(
    int BudgetId,
    string Description,
    int SubCategoryId,
    int Month,
    int Year,
    decimal Amount,
    string Type)
{
    public PeriodAllocationEntity ToEntity(BudgetType type)
     => PeriodAllocationEntity.Create(BudgetId, Description, SubCategoryId, Month, Year, Amount, type);
}

public class AttachPeriodAllocationService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result> Handle(AttachPeriodAllocationServiceInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Budget id is required");

        var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Id == input.BudgetId).ConfigureAwait(false);
        if (budget == null)
            return Result.Fail("Budget does not exist");

        if (input.Amount <= 0)
            return Result.Fail("Amount must be greater than 0");

        if (input.SubCategoryId <= 0)
            return Result.Fail("SubCategory id is required");

        if (!await _context.SubCategories.AnyAsync(s => s.Id == input.SubCategoryId).ConfigureAwait(false))
            return Result.Fail("SubCategory does not exist");

        if (string.IsNullOrWhiteSpace(input.Type))
            return Result.Fail("Type is required");

        if (!TryParseBudgetType(input.Type, out var budgetType))
            return Result.Fail("Invalid type. Allowed values: Income, Outcome, Saving");

        var periodBudget = input.ToEntity(budgetType);
        _context.Add(periodBudget);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        return Result.Ok();
    }

    private static bool TryParseBudgetType(string value, out BudgetType type)
    {
        try
        {
            type = value.DehumanizeTo<BudgetType>();
            return true;
        }
        catch
        {
            type = default;
            return false;
        }
    }
}
