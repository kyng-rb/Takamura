using FluentResults;
using Humanizer;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Base.Enums;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.BudgetAllocation;

namespace Takamura.Application.Features.Budget.Create;

public class CreateBudgetService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(CreateBudgetServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return Result.Fail<int>("Title is required");
        
        if (input.Amount <= 0)
            return Result.Fail<int>("Amount must be greater than 0");

        for (var i = 0; i < input.Allocations.Length; i++)
        {
            var allocation = input.Allocations[i];
            
            if (allocation.Amount <= 0)
                return Result.Fail<int>($"Allocation amount must be greater than 0 on the {i.Ordinalize()} allocation");
            
            if (allocation.SubCategoryId <= 0)
                return Result.Fail<int>($"SubCategory id is required on the {i.Ordinalize()} allocation");
            
            if (!_context.SubCategories.Any(s => s.Id == allocation.SubCategoryId))
                return Result.Fail<int>($"SubCategory does not exist on the {i.Ordinalize()} allocation");
        }

        var entity = input.ToEntity();
        _context.Add(entity);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Result.Ok(entity.Id);
    }
}

public record CreateBudgetServiceInput(string Title, decimal Amount, BudgetAllocationInput[] Allocations)
{
    public BudgetEntity ToEntity()
    {
        return new BudgetEntity
        {
            Id = 0,
            Title = Title,
            BudgetAllocations = Allocations.Select(allocation => allocation.ToEntity(0))
                .ToList(),
            CreatedAt = DateTime.UtcNow,
            Status = Status.Created
        };
    }
}

public record BudgetAllocationInput(int SubCategoryId, DateOnly From, DateOnly To, decimal Amount)
{
    internal BudgetAllocationEntity ToEntity(int budgetId)
    {
        return new BudgetAllocationEntity
        {
            Id = 0,
            Amount = Amount,
            From = From,
            To = To,
            SubCategoryId = SubCategoryId,
            BudgetId = budgetId,
            CreatedAt = DateTime.UtcNow,
            Status = Status.Created
        };
    }
}