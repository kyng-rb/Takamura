using Takamura.Application.Database.Enums;

namespace Takamura.Application.Database.Entities;

public class BudgetAllocation
{
    public required int Id { get; set; }
    public required decimal Amount { get; set; }
    public required Status State { get; set; }
    public required DateOnly From { get; set; }
    public required DateOnly To { get; set; }
    
    public required int SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }
    
    public required int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}