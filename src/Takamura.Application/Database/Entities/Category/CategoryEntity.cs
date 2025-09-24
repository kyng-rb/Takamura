using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database.Entities.Category;

[Table("Category")]
public class CategoryEntity : EntityBase
{
    public required string Description { get; set; }

    public List<SubCategoryEntity> SubCategories { get; set; } = [];

    public static CategoryEntity Create(string description)
        => new()
        {
            Id = 0,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            Status = Base.Enums.Status.Created
        };

    public void Update(string description)
    {
        Description = description;
        LastUpdateAt = DateTime.UtcNow;
        Status = Base.Enums.Status.Updated;
    }

    public void Delete()
    {
        LastUpdateAt = DateTime.UtcNow;
        Status = Base.Enums.Status.Deleted;
    }

    public void AttachSubCategory(string description)
    {
        var subCategory = SubCategoryEntity.Create(Id, description);
        SubCategories.Add(subCategory);
    }

    public void AttachSubCategories(IEnumerable<string> descriptions)
    {
        foreach (var description in descriptions)
        {
            var subCategory = SubCategoryEntity.Create(Id, description);
            SubCategories.Add(subCategory);
        }
    }
}