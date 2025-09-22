using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.Bill;

[Table("Bill")]
public class BillEntity : EntityBase
{
    public required DateOnly Date { get; set; }

    public required decimal Amount { get; set; }

    public required string Description { get; set; }

    public required int SubCategoryId { get; set; }

    public SubCategoryEntity? SubCategory { get; set; }

    public required int BudgetId { get; set; }

    public BudgetEntity Budget { get; set; } = null!;
}