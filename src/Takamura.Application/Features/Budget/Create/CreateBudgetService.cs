using FluentResults;
using Humanizer;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.PeriodAllocation;

namespace Takamura.Application.Features.Budget.Create;

public class CreateBudgetService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(CreateBudgetServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return Result.Fail<int>("Title is required");

        var validationResult = ValidateBudgetPeriods(input);
        if (validationResult.IsFailed)
            return Result.Fail<int>(validationResult.Errors);

        var entity = input.ToEntity();
        _context.Add(entity);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        return Result.Ok(entity.Id);
    }

    private Result ValidateBudgetPeriods(CreateBudgetServiceInput input)
    {
        if (input.Allocations is null || input.Allocations.Length == 0)
            return Result.Ok();

        for (var i = 0; i < input.Allocations.Length; i++)
        {
            var allocation = input.Allocations[i];

            if (allocation.Amount <= 0)
                return Result.Fail($"Allocation amount must be greater than 0 on the {i + 1.Ordinalize()} allocation");

            if (allocation.SubCategoryId <= 0)
                return Result.Fail($"SubCategory id is required on the {i + 1.Ordinalize()} allocation");

            if (!_context.SubCategories.Any(s => s.Id == allocation.SubCategoryId))
                return Result.Fail($"SubCategory does not exist on the {i + 1.Ordinalize()} allocation");
        }

        return Result.Ok();
    }
}

public record CreateBudgetServiceInput(
    string Title,
    CreateBudgetPeriodAllocationInput[] Allocations)
{
    public BudgetEntity ToEntity()
    {
        var budget = BudgetEntity.Create(Title);

        if (Allocations is null || Allocations.Length == 0)
            return budget;

        foreach (var allocation in Allocations)
        {
            var periodBudget = PeriodAllocationEntity.Create(budget.Id,
                allocation.SubCategoryId,
                allocation.MonthFrom,
                allocation.YearFrom,
                allocation.Description,
                allocation.Amount,
                allocation.Type);
            budget.AddPeriodBudget(periodBudget);
        }

        return budget;
    }
}

public record CreateBudgetPeriodAllocationInput(
    int SubCategoryId,
    int MonthFrom,
    int YearFrom,
    string Description,
    decimal Amount,
    string Type);