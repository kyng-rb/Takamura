using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Database.Entities.Budget;

public class BudgetEntityConfiguration : IEntityTypeConfiguration<BudgetEntity>
{
    public void Configure(EntityTypeBuilder<BudgetEntity> builder)
    {
        builder.HasIndex(b => b.Title)
            .IsUnique()
            .HasFilter(UniqueIndex.NotDeletedFilter);
    }
}
