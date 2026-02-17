using FluentResults;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.PeriodBudget;

namespace Takamura.Application.Features.Budget.Create;

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
            var periodBudget = PeriodAllocationEntity.Create(
                budget.Id,
                allocation.Description,
                allocation.SubCategoryId,
                allocation.MonthFrom,
                allocation.YearFrom,
                allocation.Amount,
                allocation.ToBudgetType());
            budget.AddPeriodBudget(periodBudget);
        }

        return budget;
    }
}

public record CreateBudgetPeriodAllocationInput(
    string Description,
    int SubCategoryId,
    int MonthFrom,
    int YearFrom,
    decimal Amount,
    string Type)
{
    public BudgetType ToBudgetType() => Type.DehumanizeTo<BudgetType>();
}

public class CreateBudgetService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result> Handle(CreateBudgetServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return Result.Fail("Title is required");

        var validationResult = await ValidateBudgetPeriods(input).ConfigureAwait(false);
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var entity = input.ToEntity();
        _context.Add(entity);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        return Result.Ok();
    }

    private async Task<Result> ValidateBudgetPeriods(CreateBudgetServiceInput input)
    {
        if (input.Allocations is null || input.Allocations.Length == 0)
            return Result.Ok();

        var subCategoryIndexById = new Dictionary<int, int>();

        for (var i = 0; i < input.Allocations.Length; i++)
        {
            var allocation = input.Allocations[i];

            if (allocation.Amount <= 0)
                return Result.Fail($"Allocation amount must be greater than 0 on the {i + 1.Ordinalize()} allocation");

            if (allocation.SubCategoryId <= 0)
                return Result.Fail($"SubCategory id is required on the {i + 1.Ordinalize()} allocation");

            if (!subCategoryIndexById.ContainsKey(allocation.SubCategoryId))
                subCategoryIndexById[allocation.SubCategoryId] = i;

            if (string.IsNullOrWhiteSpace(allocation.Type))
                return Result.Fail($"Type is required on the {i + 1.Ordinalize()} allocation");

            if (!TryParseBudgetType(allocation.Type, out _))
                return Result.Fail($"Invalid type on the {i + 1.Ordinalize()} allocation. Allowed values: Income, Outcome, Saving");
        }

        var subCategoryIds = subCategoryIndexById.Keys.ToArray();
        var existingSubCategoryIds = await _context.SubCategories
            .Where(s => subCategoryIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync()
            .ConfigureAwait(false);
        var existingSubCategorySet = existingSubCategoryIds.ToHashSet();

        foreach (var (subCategoryId, index) in subCategoryIndexById)
        {
            if (!existingSubCategorySet.Contains(subCategoryId))
                return Result.Fail($"SubCategory does not exist on the {index + 1.Ordinalize()} allocation");
        }

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
