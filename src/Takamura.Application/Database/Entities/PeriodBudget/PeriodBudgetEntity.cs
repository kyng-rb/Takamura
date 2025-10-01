using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.PeriodBudget;

[Table("PeriodBudget")]
public class PeriodAllocationEntity : EntityBase
{
    public required decimal Amount { get; set; }

    public required int MonthFrom { get; set; }

    public required int YearFrom { get; set; }

    public required BudgetType Type { get; set; }

    public required int SubCategoryId { get; set; }

    public SubCategoryEntity? SubCategory { get; set; }

    public required int BudgetId { get; set; }

    public BudgetEntity Budget { get; set; } = null!;

    public static PeriodAllocationEntity Create(int budgetId,
                                            int subCategoryId,
                                            int month,
                                            int year,
                                            decimal amount,
                                            string type)
    {
        return new PeriodAllocationEntity
        {
            Id = 0,
            BudgetId = budgetId,
            SubCategoryId = subCategoryId,
            MonthFrom = month,
            YearFrom = year,
            Amount = amount,
            Type = type.DehumanizeTo<BudgetType>(),
            CreatedAt = DateTime.UtcNow,
            Status = Base.Enums.Status.Created
        };
    }
}