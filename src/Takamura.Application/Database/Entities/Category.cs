using Takamura.Application.Database.Enums;

namespace Takamura.Application.Database.Entities;

public class Category
{
    public required int Id { get; set; }
    public required string Description { get; set; }
    public required Status State { get; set; }

    public List<SubCategory>? SubCategories { get; set; }
}