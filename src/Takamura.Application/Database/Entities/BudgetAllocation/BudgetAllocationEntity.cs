using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.BudgetAllocation;

[Table("BudgetAllocation")]
public class BudgetAllocationEntity : EntityBase
{
    public required decimal Amount { get; set; }
    public required DateOnly From { get; set; }
    public required DateOnly To { get; set; }
    
    public required int SubCategoryId { get; set; }
    public SubCategoryEntity? SubCategory { get; set; }
    
    public required int BudgetId { get; set; }
    public BudgetEntity? Budget { get; set; }
}