using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.Category;

[Table("Category")]
public class CategoryEntity : EntityBase
{
    public required string Description { get; set; }

    public List<SubCategoryEntity> SubCategories { get; set; } = [];
}