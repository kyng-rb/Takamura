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

public record CreateBillOutput(int Id);

public class CreateBillService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<CreateBillOutput>> Handle(CreateBillServiceInput input, CancellationToken cancellationToken = default)
    {
        // A missing "date" in the JSON binds to DateOnly's default (0001-01-01).
        if (input.Date == default)
            return Result.Fail("Date is required");

        if (input.Date.Year is < FieldLimits.MinYear or > FieldLimits.MaxYear)
            return Result.Fail($"Date must be between {FieldLimits.MinYear} and {FieldLimits.MaxYear}");

        if (input.Amount <= 0)
            return Result.Fail("Amount must be greater than 0");

        if (decimal.Round(input.Amount, FieldLimits.MoneyDecimals) != input.Amount)
            return Result.Fail($"Amount can have at most {FieldLimits.MoneyDecimals} decimal places");

        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Description is required");

        if (input.Description.Length > FieldLimits.TextMaxLength)
            return Result.Fail($"Description must be at most {FieldLimits.TextMaxLength} characters");

        if (input.SubCategoryId <= 0)
            return Result.Fail("SubCategory id is required");

        if (input.BudgetId <= 0)
            return Result.Fail("Budget id is required");

        if (!await _context.SubCategories.AnyAsync(x => x.Id == input.SubCategoryId, cancellationToken).ConfigureAwait(false))
            return Result.Fail("SubCategory does not exist");

        if (!await _context.Budgets.AnyAsync(x => x.Id == input.BudgetId, cancellationToken).ConfigureAwait(false))
            return Result.Fail("Budget does not exist");

        var entity = input.ToEntity();

        _context.Add(entity);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Ok(new CreateBillOutput(entity.Id));
    }
}
