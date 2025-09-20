using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Takamura.Application.Database.Entities.SubCategory;

public class SubCategoryEntityConfiguration: IEntityTypeConfiguration<SubCategoryEntity>
{
    public void Configure(EntityTypeBuilder<SubCategoryEntity> builder)
    {
        builder.HasOne(sc => sc.Category)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(sc => sc.CategoryId);
    }
}