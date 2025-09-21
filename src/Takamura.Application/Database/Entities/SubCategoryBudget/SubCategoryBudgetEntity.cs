using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.MonthlyBudget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.SubCategoryBudget;

[Table("SubCategoryBudget")]
public class SubCategoryBudgetEntity : EntityBase
{
    public required decimal Amount { get; set; }

    public required int MonthFrom { get; set; }

    public required int YearFrom { get; set; }

    public required int SubCategoryId { get; set; }

    public SubCategoryEntity? SubCategory { get; set; }


    public required int MonthlyBudgetId { get; set; }


    public MonthlyBudgetEntity? MonthlyBudget { get; set; }
}