namespace Takamura.Application.Features.Budget.Summary;

public class YearToDateBalance
{
    public string Budget { get; set; } = null!;

    public decimal PeriodBudget { get; set; }

    public decimal AvailableAmount { get; set; }

    public string Category { get; set; } = null!;

    public string SubCategory { get; set; } = null!;
}