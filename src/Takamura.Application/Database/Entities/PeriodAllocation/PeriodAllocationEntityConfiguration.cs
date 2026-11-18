using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Database.Entities.PeriodAllocation;

public class PeriodAllocationEntityConfiguration : IEntityTypeConfiguration<PeriodAllocationEntity>
{
    public void Configure(EntityTypeBuilder<PeriodAllocationEntity> builder)
    {
        // One allocation per subcategory and month in a budget, whatever its type.
        builder.HasIndex(a => new { a.BudgetId, a.SubCategoryId, a.YearFrom, a.MonthFrom })
            .IsUnique()
            .HasFilter(UniqueIndex.NotDeletedFilter);
    }
}
