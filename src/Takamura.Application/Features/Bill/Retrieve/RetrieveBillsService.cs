using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Bill;

namespace Takamura.Application.Features.Bill.Retrieve;

public class RetrieveBillsService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrieveBillsOutput>> Handle(RetrieveBillsOptions options)
    {
        if (options.BudgetId <= 0)
            return Result.Fail("Budget Id is required");
        
        if (options.From is not null && options.To is not null && options.From < options.To)
            return Result.Fail("From must be greater than To");
        
        if (!_context.Budgets.Any(budget => budget.Id == options.BudgetId))
            return Result.Fail("Budget does not exist");
        
        if (options.CategoryId is not null && !_context.Categories.Any(category => category.Id == options.CategoryId))
            return Result.Fail("Category does not exist");
        
        if (options.SubCategoryId is not null && !_context.SubCategories.Any(subcategory => subcategory.Id == options.SubCategoryId))
            return Result.Fail("SubCategory does not exist");

        var query = _context.Bills.Where(bill => bill.BudgetId == options.BudgetId);
        
        if (options.From is not null)
            query = query.Where(bill => bill.Date >= options.From);
        
        if (options.To is not null)
            query = query.Where(bill => bill.Date <= options.To);
        
        if (options.SubCategoryId is not null)
            query = query.Where(bill => bill.SubCategoryId == options.SubCategoryId);
        
        if (options.CategoryId is not null)
            query = query.Where(bill => bill.SubCategory!.CategoryId == options.CategoryId);
        
        query = query.Include(s => s.SubCategory!.CategoryEntity)
            .Include(x => x.SubCategory)
            .OrderBy(w => w.Date);
        
        var bills = await query.ToListAsync().ConfigureAwait(false);

        var outputBills = bills.Select(RetrieveSingleBillOutput.FromEntity);
        return Result.Ok(new RetrieveBillsOutput(outputBills));
    }
}

public record RetrieveBillsOptions(int BudgetId, DateOnly? From, DateOnly? To, int? CategoryId, int? SubCategoryId);

public record RetrieveSingleBillOutput(
    int Id,
    string Description,
    decimal Amount,
    DateOnly Date,
    int BudgetId,
    string Category,
    string Subcategory)
{
    internal static RetrieveSingleBillOutput FromEntity(BillEntity billEntity)
    {
        return new RetrieveSingleBillOutput(billEntity.Id,
            billEntity.Description,
            billEntity.Amount,
            billEntity.Date,
            billEntity.BudgetId,
            billEntity.SubCategory!.CategoryEntity.Description,
            billEntity.SubCategory.Description);
    }
}

public record RetrieveBillsOutput(IEnumerable<RetrieveSingleBillOutput> Bills);