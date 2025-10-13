using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Summary;

namespace Takamura.Application.Features.Budget.Summary;

public class RetrieveMonthlyCategoryBalanceService(DatabaseContext dbContext)
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<Result<RetrieveMonthlyBalanceServiceOutput>> Handle(RetrieveMonthlyCategoryBalanceServiceInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail(new Error("Invalid BudgetId"));

        if (input.Year < 2025)
            return Result.Fail(new Error("Invalid Year"));

        if (input.Month.HasValue && (input.Month < 1 || input.Month > 12))
            return Result.Fail(new Error("Invalid Month"));

        var records = await _dbContext.GetMonthlyCategoryBalance(input.BudgetId, input.Year, input.Month)
            .ToListAsync()
            .ConfigureAwait(false);

        var output = new RetrieveMonthlyBalanceServiceOutput(records);

        return Result.Ok(output);
    }
}

public record RetrieveMonthlyBalanceServiceOutput(IEnumerable<MonthlyCategoryBalance> Records);

public record RetrieveMonthlyCategoryBalanceServiceInput(int BudgetId, int Year, int? Month);