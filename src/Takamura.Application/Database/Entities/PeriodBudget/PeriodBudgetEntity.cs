using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.PeriodBudget;

[Table("PeriodBudget")]
public class PeriodBudgetEntity : EntityBase
{
    public required decimal Amount { get; set; }

    public required int MonthFrom { get; set; }

    public required int YearFrom { get; set; }

    public required BudgetType Type { get; set; }

    public required int SubCategoryId { get; set; }

    public SubCategoryEntity? SubCategory { get; set; }

    public required int BudgetId { get; set; }

    public BudgetEntity Budget { get; set; } = null!;
}