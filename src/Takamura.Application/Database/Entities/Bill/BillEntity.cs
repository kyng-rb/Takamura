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

    public SubCategoryEntity SubCategory { get; set; } = null!;

    public required int BudgetId { get; set; }

    public BudgetEntity Budget { get; set; } = null!;

    public static BillEntity Create(
        DateOnly date,
        decimal amount,
        string description,
        int subCategoryId,
        int budgetId)
    {
        return new BillEntity
        {
            Id = 0,
            BudgetId = budgetId,
            Date = date,
            Amount = amount,
            Description = description,
            SubCategoryId = subCategoryId,
            CreatedAt = DateTime.UtcNow,
            Status = Base.Enums.Status.Created
        };
    }
}