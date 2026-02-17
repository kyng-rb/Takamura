using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.PeriodBudget;

[Table("PeriodAllocation")]
public class PeriodAllocationEntity : EntityBase
{
    public required decimal Amount { get; set; }

    public required string Description { get; set; }

    public required int MonthFrom { get; set; }

    public required int YearFrom { get; set; }

    public required BudgetType Type { get; set; }

    public required int SubCategoryId { get; set; }

    public SubCategoryEntity? SubCategory { get; set; }

    public required int BudgetId { get; set; }

    public BudgetEntity Budget { get; set; } = null!;

    public static PeriodAllocationEntity Create(
        int budgetId,
        string budgetDescription,
        int subCategoryId,
        int month,
        int year,
        decimal amount,
        BudgetType type)
    {
        return new PeriodAllocationEntity
        {
            Id = 0,
            Description = budgetDescription,
            BudgetId = budgetId,
            SubCategoryId = subCategoryId,
            MonthFrom = month,
            YearFrom = year,
            Amount = amount,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            Status = Base.Enums.Status.Created
        };
    }
}
