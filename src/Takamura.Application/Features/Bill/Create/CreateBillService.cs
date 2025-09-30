using FluentResults;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Bill;

namespace Takamura.Application.Features.Bill.Create;

public class CreateBillService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(CreateBillServiceInput input)
    {
        if (input.Amount <= 0)
            return Result.Fail<int>("Amount must be greater than 0");

        if (string.IsNullOrEmpty(input.Description))
            return Result.Fail<int>("Description must not be empty");

        if (input.SubCategoryId <= 0)
            return Result.Fail<int>("SubCategoryId is required");

        if (input.BudgetId <= 0)
            return Result.Fail<int>("BudgetId is required");

        if (!_context.SubCategories.Any(x => x.Id == input.SubCategoryId))
            return Result.Fail<int>("SubCategory does not exist");

        if (!_context.Budgets.Any(x => x.Id == input.BudgetId))
            return Result.Fail<int>("Budget does not exist");

        var entity = input.ToEntity();

        _context.Add(entity);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok(entity.Id);
    }
}

public record CreateBillServiceInput(DateOnly Date, decimal Amount, string Description, int BudgetId, int SubCategoryId)
{
    public BillEntity ToEntity()
        => BillEntity.Create(Date, Amount, Description, SubCategoryId, BudgetId);
}