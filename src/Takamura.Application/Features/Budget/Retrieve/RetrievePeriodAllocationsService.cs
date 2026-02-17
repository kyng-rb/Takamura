using FluentResults;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.PeriodBudget;

namespace Takamura.Application.Features.Budget.Retrieve;

public record RetrievePeriodAllocationsOutput(IEnumerable<PeriodAllocationOutput> Allocations)
{
    public static RetrievePeriodAllocationsOutput FromEntities(IEnumerable<PeriodAllocationEntity> allocations)
    {
        var outputs = allocations.Select(x => new PeriodAllocationOutput(
            x.Id,
            x.Description,
            x.Amount,
            x.MonthFrom,
            x.YearFrom,
            x.Type.Humanize(),
            x.SubCategoryId,
            x.SubCategory!.Description,
            x.SubCategory!.CategoryId,
            x.SubCategory!.Category.Description
        ));

        return new RetrievePeriodAllocationsOutput(outputs);
    }
}

public record PeriodAllocationOutput(
    int Id,
    string Description,
    decimal Amount,
    int Month,
    int Year,
    string Type,
    int SubCategoryId,
    string SubCategory,
    int CategoryId,
    string Category);

public record RetrievePeriodAllocationInput(int BudgetId, int? Year, int? Month, int? SubCategoryId, int? CategoryId, string? Type);

public class RetrievePeriodAllocationsService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrievePeriodAllocationsOutput>> Handle(RetrievePeriodAllocationInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Invalid budget id");

        if (input.Month.HasValue && (input.Month < 1 || input.Month > 12))
            return Result.Fail("Invalid month");

        BudgetType? typeFilter = null;
        if (!string.IsNullOrWhiteSpace(input.Type))
        {
            if (!TryParseBudgetType(input.Type, out var budgetType))
                return Result.Fail("Invalid type. Allowed values: Income, Outcome, Saving");

            typeFilter = budgetType;
        }

        var allocations = await GetRecords(input, typeFilter).ConfigureAwait(false);

        var output = RetrievePeriodAllocationsOutput.FromEntities(allocations);

        return Result.Ok(output);
    }

    private async Task<List<PeriodAllocationEntity>> GetRecords(RetrievePeriodAllocationInput input, BudgetType? typeFilter)
    {
        var allocations = _context.PeriodAllocations
                    .Where(x => x.BudgetId == input.BudgetId);

        if (input.Year.HasValue)
            allocations = allocations.Where(x => x.YearFrom == input.Year.Value);

        if (input.Month.HasValue)
            allocations = allocations.Where(x => x.MonthFrom == input.Month.Value);

        if (input.SubCategoryId.HasValue)
            allocations = allocations.Where(x => x.SubCategoryId == input.SubCategoryId.Value);

        if (input.CategoryId.HasValue)
            allocations = allocations.Where(x => x.SubCategory!.CategoryId == input.CategoryId.Value);

        if (typeFilter.HasValue)
            allocations = allocations.Where(x => x.Type == typeFilter.Value);

        return await allocations
            .Include(s => s.SubCategory)
            .ThenInclude(c => c!.Category)
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);
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
