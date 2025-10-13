using FluentResults;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.PeriodAllocation;

namespace Takamura.Application.Features.Budget.Retrieve;

public class RetrievePeriodAllocationsService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrievePeriodAllocationsOutput>> Handle(RetrievePeriodAllocationInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail("Invalid budget id");

        if (input.Month.HasValue && (input.Month < 1 || input.Month > 12))
            return Result.Fail("Invalid month");

        var allocations = await GetRecords(input).ConfigureAwait(false);

        var output = RetrievePeriodAllocationsOutput.FromEntities(allocations);

        return Result.Ok(output);
    }

    private async Task<List<PeriodAllocationEntity>> GetRecords(RetrievePeriodAllocationInput input)
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

        if (!string.IsNullOrWhiteSpace(input.Type))
            allocations = allocations.Where(x => x.Type == input.Type.DehumanizeTo<AllocationType>());

        return await allocations
            .Include(s => s.SubCategory)
            .ThenInclude(c => c!.Category)
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);
    }
}

public record RetrievePeriodAllocationsOutput(IEnumerable<PeriodAllocationOutput> Allocations)
{
    public static RetrievePeriodAllocationsOutput FromEntities(IEnumerable<PeriodAllocationEntity> allocations)
    {
        var outputs = allocations.Select(x => new PeriodAllocationOutput(
            x.Id,
            x.Amount,
            x.MonthFrom,
            x.YearFrom,
            x.Type.Humanize(),
            x.Description,
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
    decimal Amount,
    int Month,
    int Year,
    string Type,
    string Description,
    int SubCategoryId,
    string SubCategory,
    int CategoryId,
    string Category);

public record RetrievePeriodAllocationInput(int BudgetId, int? Year, int? Month, int? SubCategoryId, int? CategoryId, string? Type);