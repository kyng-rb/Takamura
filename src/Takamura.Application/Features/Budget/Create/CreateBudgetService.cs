using System.Globalization;
using FluentResults;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.PeriodAllocation;

namespace Takamura.Application.Features.Budget.Create;

public record CreateBudgetServiceInput(
    string Title,
    CreateBudgetPeriodAllocationInput[] Allocations)
{
    public BudgetEntity ToEntity()
    {
        var budget = BudgetEntity.Create(Title);

        foreach (var allocation in Allocations)
        {
            var entity = PeriodAllocationEntity.Create(
                budget.Id,
                allocation.Description,
                allocation.SubCategoryId,
                allocation.MonthFrom,
                allocation.YearFrom,
                allocation.Amount,
                allocation.ToBudgetType());
            budget.AddPeriodAllocation(entity);
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
    public BudgetType ToBudgetType() => BudgetTypeParser.Parse(Type);
}

public record CreateBudgetOutput(int Id);

public class CreateBudgetService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<CreateBudgetOutput>> Handle(CreateBudgetServiceInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return Result.Fail("Title is required");

        if (input.Title.Length > FieldLimits.TextMaxLength)
            return Result.Fail($"Title must be at most {FieldLimits.TextMaxLength} characters");

        if (await _context.Budgets.AnyAsync(x => x.Title == input.Title, cancellationToken).ConfigureAwait(false))
            return Result.Fail("A budget with this title already exists");

        var validationResult = await ValidateBudgetPeriods(input, cancellationToken).ConfigureAwait(false);
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var entity = input.ToEntity();
        _context.Add(entity);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok(new CreateBudgetOutput(entity.Id));
    }

    private static readonly CultureInfo MessageCulture = CultureInfo.GetCultureInfo("en-US");

    /// <summary>"1st", "2nd"... for a zero-based allocation index, always in English like the other messages.</summary>
    private static string Ordinal(int index) => (index + 1).Ordinalize(MessageCulture);

    private async Task<Result> ValidateBudgetPeriods(CreateBudgetServiceInput input, CancellationToken cancellationToken)
    {
        if (input.Allocations.Length == 0)
            return Result.Ok();

        var subCategoryIndexById = new Dictionary<int, int>();
        var periods = new HashSet<(int SubCategoryId, int Year, int Month)>();

        for (var i = 0; i < input.Allocations.Length; i++)
        {
            var allocation = input.Allocations[i];

            if (string.IsNullOrWhiteSpace(allocation.Description))
                return Result.Fail($"Description is required on the {Ordinal(i)} allocation");

            if (allocation.Description.Length > FieldLimits.TextMaxLength)
                return Result.Fail($"Description must be at most {FieldLimits.TextMaxLength} characters on the {Ordinal(i)} allocation");

            if (allocation.MonthFrom is < 1 or > 12)
                return Result.Fail($"Month must be between 1 and 12 on the {Ordinal(i)} allocation");

            if (allocation.YearFrom is < FieldLimits.MinYear or > FieldLimits.MaxYear)
                return Result.Fail($"Year must be between {FieldLimits.MinYear} and {FieldLimits.MaxYear} on the {Ordinal(i)} allocation");

            if (allocation.Amount <= 0)
                return Result.Fail($"Amount must be greater than 0 on the {Ordinal(i)} allocation");

            if (decimal.Round(allocation.Amount, FieldLimits.MoneyDecimals) != allocation.Amount)
                return Result.Fail($"Amount can have at most {FieldLimits.MoneyDecimals} decimal places on the {Ordinal(i)} allocation");

            if (allocation.SubCategoryId <= 0)
                return Result.Fail($"SubCategory id is required on the {Ordinal(i)} allocation");

            if (!periods.Add((allocation.SubCategoryId, allocation.YearFrom, allocation.MonthFrom)))
                return Result.Fail($"The {Ordinal(i)} allocation repeats a subcategory and month already used by another allocation");

            if (!subCategoryIndexById.ContainsKey(allocation.SubCategoryId))
                subCategoryIndexById[allocation.SubCategoryId] = i;

            if (string.IsNullOrWhiteSpace(allocation.Type))
                return Result.Fail($"Type is required on the {Ordinal(i)} allocation");

            if (!BudgetTypeParser.TryParse(allocation.Type, out _))
                return Result.Fail($"Type must be one of: {BudgetTypeParser.AllowedValues} on the {Ordinal(i)} allocation");
        }

        var subCategoryIds = subCategoryIndexById.Keys.ToArray();
        var existingSubCategoryIds = await _context.SubCategories
            .Where(s => subCategoryIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        var existingSubCategorySet = existingSubCategoryIds.ToHashSet();

        foreach (var (subCategoryId, index) in subCategoryIndexById)
        {
            if (!existingSubCategorySet.Contains(subCategoryId))
                return Result.Fail($"SubCategory does not exist on the {Ordinal(index)} allocation");
        }

        return Result.Ok();
    }
}
