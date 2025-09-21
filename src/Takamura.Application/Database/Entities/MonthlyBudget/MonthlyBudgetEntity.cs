using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.SubCategoryBudget;

namespace Takamura.Application.Database.Entities.MonthlyBudget;

[Table("MonthlyBudget")]
public class MonthlyBudgetEntity : EntityBase
{
    public required decimal Amount { get; set; }

    public required int BudgetId { get; set; }

    public BudgetEntity Budget { get; set; } = null!;

    public IEnumerable<SubCategoryBudgetEntity> SubCategoryBudgets { get; set; } = [];
}