using FluentResults;
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

public record AttachPeriodAllocationOutput(int Id);

public class AttachPeriodAllocationService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<AttachPeriodAllocationOutput>> Handle(AttachPeriodAllocationServiceInput input, CancellationToken cancellationToken = default)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Budget id is required");

        var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Id == input.BudgetId, cancellationToken).ConfigureAwait(false);
        if (budget == null)
            return Result.Fail("Budget does not exist");

        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Description is required");

        if (input.Description.Length > FieldLimits.TextMaxLength)
            return Result.Fail($"Description must be at most {FieldLimits.TextMaxLength} characters");

        if (input.Month is < 1 or > 12)
            return Result.Fail("Month must be between 1 and 12");

        if (input.Year is < FieldLimits.MinYear or > FieldLimits.MaxYear)
            return Result.Fail($"Year must be between {FieldLimits.MinYear} and {FieldLimits.MaxYear}");

        if (input.Amount <= 0)
            return Result.Fail("Amount must be greater than 0");

        if (decimal.Round(input.Amount, FieldLimits.MoneyDecimals) != input.Amount)
            return Result.Fail($"Amount can have at most {FieldLimits.MoneyDecimals} decimal places");

        if (input.SubCategoryId <= 0)
            return Result.Fail("SubCategory id is required");

        if (!await _context.SubCategories.AnyAsync(s => s.Id == input.SubCategoryId, cancellationToken).ConfigureAwait(false))
            return Result.Fail("SubCategory does not exist");

        if (string.IsNullOrWhiteSpace(input.Type))
            return Result.Fail("Type is required");

        if (!BudgetTypeParser.TryParse(input.Type, out var budgetType))
            return Result.Fail($"Invalid type. Allowed values: {BudgetTypeParser.AllowedValues}");

        var alreadyAllocated = await _context.PeriodAllocations
            .AnyAsync(x => x.BudgetId == input.BudgetId
                && x.SubCategoryId == input.SubCategoryId
                && x.YearFrom == input.Year
                && x.MonthFrom == input.Month, cancellationToken)
            .ConfigureAwait(false);
        if (alreadyAllocated)
            return Result.Fail("This subcategory already has an allocation for that month");

        var periodBudget = input.ToEntity(budgetType);
        _context.Add(periodBudget);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok(new AttachPeriodAllocationOutput(periodBudget.Id));
    }
}
