using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Takamura.Application.Database.Entities.Base.Enums;

namespace Takamura.Application.Database.Entities.Category;

public class CategorySeed : IEntityTypeConfiguration<CategoryEntity>
{
    private readonly CategoryEntity[] _categories;
    
    public CategorySeed()
    {
        var utcNow = new DateTime(2025, 09, 20);
        _categories =
        [
            new ()
            {
                Id = 1,
                Description = "Comida",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 2,
                Description = "Hipoteca",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 3,
                Description = "Mantenimiento de la casita",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 4,
                Description = "Animalijos",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 5,
                Description = "Entretenimiento personal",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 6,
                Description = "Ahorro",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 7,
                Description = "Inesperados",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            },
            new ()
            {
                Id = 8,
                Description = "Vehiculos",
                CreatedAt = utcNow,
                LastUpdateAt = null,
                Status = Status.Created,
            }
        ];
    }

    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.HasData(_categories);
    }
}