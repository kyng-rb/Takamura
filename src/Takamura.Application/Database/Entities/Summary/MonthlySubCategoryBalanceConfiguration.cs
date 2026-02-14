using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Database.Entities.Summary
{
    public class MonthlySubCategoryBalanceConfiguration : IEntityTypeConfiguration<MonthlySubCategoryBalance>
    {
        public void Configure(EntityTypeBuilder<MonthlySubCategoryBalance> builder)
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
}