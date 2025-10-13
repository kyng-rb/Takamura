using FluentResults;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.PeriodAllocation;

namespace Takamura.Application.Features.Budget.AttachPeriod;

public class AttachPeriodAllocationService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(AttachPeriodAllocationServiceInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Budget id is required");

        var budget = _context.Budgets.FirstOrDefault(b => b.Id == input.BudgetId);
        if (budget == null)
            return Result.Fail("Budget does not exist");

        if (input.Amount <= 0)
            return Result.Fail("Amount must be greater than 0");

        if (input.SubCategoryId <= 0)
            return Result.Fail("SubCategory id is required");

        if (!_context.SubCategories.Any(s => s.Id == input.SubCategoryId))
            return Result.Fail("SubCategory does not exist");

        var periodBudget = input.ToEntity();
        _context.Add(periodBudget);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        return Result.Ok(periodBudget.Id);
    }
}

public record AttachPeriodAllocationServiceInput(
    int BudgetId,
    int SubCategoryId,
    int Month,
    int Year,
    string Description,
    decimal Amount,
    string Type)
{
    public PeriodAllocationEntity ToEntity()
     => PeriodAllocationEntity.Create(BudgetId, SubCategoryId, Month, Year, Description, Amount, Type);

}