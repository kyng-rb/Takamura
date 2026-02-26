using System.ComponentModel.DataAnnotations.Schema;

namespace Takamura.Application.Database.Entities.Balance;

public class MonthlySubCategoryBalanceEntity
{
    public string Budget { get; set; } = string.Empty;

    [Column("Period_Budget")]
    public decimal PeriodBudget { get; set; }

    [Column("Available_Amount")]
    public decimal AvailableAmount { get; set; }

    [Column("Month")]
    public int Month { get; set; }

    [Column("Year")]
    public int Year { get; set; }

    public string Category { get; set; } = string.Empty;

    [Column("Sub_Category")]
    public string SubCategory { get; set; } = string.Empty;
}
