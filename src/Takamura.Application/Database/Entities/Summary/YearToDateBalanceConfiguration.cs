using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Features.Budget.Summary;

public class YearToDateBalanceConfiguration : IEntityTypeConfiguration<YearToDateBalance>
{
    public void Configure(EntityTypeBuilder<YearToDateBalance> builder)
    {
        builder
                .HasNoKey();

        builder
            .Property(s => s.PeriodBudget)
            .HasColumnName("Period_Budget");

        builder
            .Property(s => s.AvailableAmount)
            .HasColumnName("Available_Amount");

        builder
            .Property(s => s.SubCategory)
            .HasColumnName("Sub_Category");
    }
}