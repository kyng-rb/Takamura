using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Takamura.Application.Database.Entities.Base.Enums;

namespace Takamura.Application.Database.Entities.SubCategory;

public class SubCategorySeed : IEntityTypeConfiguration<SubCategoryEntity>
{
    private readonly SubCategoryEntity[] _subCategories;

    public SubCategorySeed()
    {
        var utcNow = new DateTime(2025, 09, 20);
        _subCategories = [
            new ()
            {
                Id = 1,
                CreatedAt = utcNow,
                Description = "Comida",
                CategoryId = 1,
                Status = Status.Created
            },
            new ()
            {
                Id = 2,
                CreatedAt = utcNow,
                Description = "Hipoteca mensual",
                CategoryId = 2,
                Status = Status.Created
            },
            new ()
            {
                Id = 3,
                CreatedAt = utcNow,
                Description = "Electricidad",
                CategoryId = 3,
                Status = Status.Created
            },
            new ()
            {
                Id = 4,
                CreatedAt = utcNow,
                Description = "Mantenimiento",
                CategoryId = 3,
                Status = Status.Created
            },
            new ()
            {
                Id = 5,
                CreatedAt = utcNow,
                Description = "Limpieza",
                CategoryId = 3,
                Status = Status.Created
            },
            new ()
            {
                Id = 6,
                CreatedAt = utcNow,
                Description = "Animalijos",
                CategoryId = 4,
                Status = Status.Created
            },
            new ()
            {
                Id = 7,
                CreatedAt = utcNow,
                Description = "Gasolina",
                CategoryId = 8,
                Status = Status.Created
            },
            new ()
            {
                Id = 8,
                CreatedAt = utcNow,
                Description = "Lavado",
                CategoryId = 8,
                Status = Status.Created
            },
            new ()
            {
                Id = 9,
                CreatedAt = utcNow,
                Description = "Reynaldo",
                CategoryId = 5,
                Status = Status.Created
            },
            new ()
            {
                Id = 10,
                CreatedAt = utcNow,
                Description = "Jenifer",
                CategoryId = 5,
                Status = Status.Created
            },
            new ()
            {
                Id = 11,
                CreatedAt = utcNow,
                Description = "Internet residencial",
                CategoryId = 3,
                Status = Status.Created
            },
            new ()
            {
                Id = 12,
                CreatedAt = utcNow,
                Description = "Ahorro principal",
                CategoryId = 6,
                Status = Status.Created
            },
            new ()
            {
                Id = 13,
                CreatedAt = utcNow,
                Description = "Ayuda a los viejos",
                CategoryId = 6,
                Status = Status.Created
            },
            new ()
            {
                Id = 14,
                CreatedAt = utcNow,
                Description = "Hipoteca adelantada",
                CategoryId = 2,
                Status = Status.Created
            },
            new ()
            {
                Id = 15,
                CreatedAt = utcNow,
                Description = "Inesperado",
                CategoryId = 7,
                Status = Status.Created
            },
        ];
    }

    public void Configure(EntityTypeBuilder<SubCategoryEntity> builder)
    {
        builder.HasData(_subCategories);
    }
}