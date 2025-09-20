using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Features.Category.Retrieve;

public class RetrieveCategoriesService(DatabaseContext context)
{
    private readonly DatabaseContext  _context = context;

    public async Task<Result<IEnumerable<Category>>> Handle()
    {
        var records = await _context
            .Categories
            .Include(x => x.SubCategories)
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);
        
        var categories = records.Select(Category.FromEntity);

        return Result.Ok(categories);
    }
}

public record SubCategory(int Id, string Description);

public record Category(int Id, string Description, SubCategory[] SubCategories)
{
    public static Category FromEntity(CategoryEntity entity)
    {
        var subCategories = entity.SubCategories.Select(x => new SubCategory(x.Id, x.Description)).ToArray();
        return new Category(entity.Id, entity.Description, subCategories);
    }
};