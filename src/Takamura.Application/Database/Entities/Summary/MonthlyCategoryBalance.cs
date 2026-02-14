namespace Takamura.Application.Database.Entities.Summary
{
    public class MonthlyCategoryBalance
    {
        public string Budget { get; set; } = null!;

        public decimal PeriodBudget { get; set; }

        public decimal AvailableAmount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string Category { get; set; } = null!;
    }
}