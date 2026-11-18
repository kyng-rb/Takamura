using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Database.Entities.Category;

public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.HasIndex(c => c.Description)
            .IsUnique()
            .HasFilter(UniqueIndex.NotDeletedFilter);
    }
}
