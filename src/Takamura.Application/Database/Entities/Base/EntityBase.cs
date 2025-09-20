using Takamura.Application.Database.Entities.Base.Enums;

namespace Takamura.Application.Database.Entities.Base;

public abstract class EntityBase
{
    public required int Id { get; set; }
    
    public required Status Status { get; set; }
    
    public required DateTime CreatedAt { get; set; }
    
    public DateTime? LastUpdateAt { get; set; }
}