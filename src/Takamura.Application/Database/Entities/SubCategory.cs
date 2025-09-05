using Takamura.Application.Database.Enums;

namespace Takamura.Application.Database.Entities;

public class SubCategory
{
    public required int Id { get; set; }
    public required string Description { get; set; }
    public required Status State { get; set; }
    
    public required int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}