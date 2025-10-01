using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.PeriodBudget;

namespace Takamura.Application.Database.Entities.Budget;

[Table("Budget")]
public class BudgetEntity : EntityBase
{
    public required string Title { get; set; }

    public List<PeriodAllocationEntity> PeriodBudgets { get; set; } = [];

    public static BudgetEntity Create(string title)
    {
        return new BudgetEntity
        {
            Id = 0,
            Title = title,
            CreatedAt = DateTime.UtcNow,
            Status = Base.Enums.Status.Created
        };
    }

    public void AddPeriodBudget(PeriodAllocationEntity periodBudget)
    {
        PeriodBudgets.Add(periodBudget);
    }
}