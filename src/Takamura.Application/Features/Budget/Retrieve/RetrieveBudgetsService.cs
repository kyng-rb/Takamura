using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Budget;

namespace Takamura.Application.Features.Budget.Retrieve;

public record BudgetOutput(int Id, string Title);

public record RetrieveBudgetsOutput(IEnumerable<BudgetOutput> Budgets)
{
    public static RetrieveBudgetsOutput FromEntities(IEnumerable<BudgetEntity> budgets)
    {
        var outputs = budgets.Select(x => new BudgetOutput(x.Id, x.Title));

        return new RetrieveBudgetsOutput(outputs);
    }
}

public class RetrieveBudgetsService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrieveBudgetsOutput>> Handle()
    {
        var budgets = await _context.Budgets
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

        return Result.Ok(RetrieveBudgetsOutput.FromEntities(budgets));
    }
}
