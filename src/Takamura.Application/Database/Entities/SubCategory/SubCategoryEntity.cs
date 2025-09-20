using System.ComponentModel.DataAnnotations.Schema;
using Takamura.Application.Database.Entities.Base;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Database.Entities.SubCategory;

[Table("SubCategory")]
public class SubCategoryEntity  : EntityBase
{
    public required string Description { get; set; }
    
    public required Type MovementType { get; set; }
    
    public required int CategoryId { get; set; }
    
    public CategoryEntity Category { get; set; } = null!;
}