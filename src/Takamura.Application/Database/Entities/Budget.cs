using Takamura.Application.Database.Enums;

namespace Takamura.Application.Database.Entities;

public class Budget
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required decimal Amount { get; set; }
    public required Status State { get; set; }
    
    public List<BudgetAllocation> BudgetAllocations { get; set; } = null!;
}