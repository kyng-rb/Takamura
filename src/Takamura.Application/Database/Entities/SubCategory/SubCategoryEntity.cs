using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Database.Entities.SubCategory;

[Table("SubCategory")]
public class SubCategoryEntity : EntityBase
{
    public required string Description { get; set; }

    public required int CategoryId { get; set; }

    public CategoryEntity Category { get; set; } = null!;

    internal static SubCategoryEntity Create(int categoryId, string description)
    {
        return new()
        {
            Id = 0,
            CategoryId = categoryId,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            Status = Base.Enums.Status.Created
        };
    }
}