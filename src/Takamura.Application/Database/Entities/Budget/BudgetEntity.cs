using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.PeriodBudget;

namespace Takamura.Application.Database.Entities.Budget;

[Table("Budget")]
public class BudgetEntity : EntityBase
{
    public required string Title { get; set; }

    public List<PeriodBudgetEntity> PeriodBudgets { get; set; } = [];
}