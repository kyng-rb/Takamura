using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Database.Entities.Summary
{
    public class MonthlyAllocationBalanceConfiguration : IEntityTypeConfiguration<MonthlyCategoryBalance>
    {

        public void Configure(EntityTypeBuilder<MonthlyCategoryBalance> builder)
        {
            builder
                .HasNoKey();

            builder
                .Property(s => s.PeriodBudget)
                .HasColumnName("Period_Budget");

            builder
                .Property(s => s.AvailableAmount)
                .HasColumnName("Available_Amount");
        }
    }
}