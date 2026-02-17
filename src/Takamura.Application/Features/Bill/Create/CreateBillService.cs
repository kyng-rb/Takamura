using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Bill;

namespace Takamura.Application.Features.Bill.Create;

public record CreateBillServiceInput(DateOnly Date, decimal Amount, string Description, int BudgetId, int SubCategoryId)
{
    public BillEntity ToEntity()
        => BillEntity.Create(Date, Amount, Description, SubCategoryId, BudgetId);
}

public class CreateBillService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result> Handle(CreateBillServiceInput input)
    {
        if (input.Amount <= 0)
            return Result.Fail("Amount must be greater than 0");

        if (string.IsNullOrEmpty(input.Description))
            return Result.Fail("Description must not be empty");

        if (input.SubCategoryId <= 0)
            return Result.Fail("SubCategoryId is required");

        if (input.BudgetId <= 0)
            return Result.Fail("BudgetId is required");

        if (!await _context.SubCategories.AnyAsync(x => x.Id == input.SubCategoryId).ConfigureAwait(false))
            return Result.Fail("SubCategory does not exist");

        if (!await _context.Budgets.AnyAsync(x => x.Id == input.BudgetId).ConfigureAwait(false))
            return Result.Fail("Budget does not exist");

        var entity = input.ToEntity();

        _context.Add(entity);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok();
    }
}
