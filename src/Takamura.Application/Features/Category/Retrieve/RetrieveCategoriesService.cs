using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Features.Category.Retrieve;

public record RetrieveCategoriesOutput(IEnumerable<Category> Categories);

public record SubCategory(int Id, string Description);

public record Category(int Id, string Description, IEnumerable<SubCategory> SubCategories)
{
    public static Category FromEntity(CategoryEntity entity)
    {
        var subCategories = entity.SubCategories.Select(x => new SubCategory(x.Id, x.Description));
        return new Category(entity.Id, entity.Description, subCategories);
    }
}

public class RetrieveCategoriesService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrieveCategoriesOutput>> Handle()
    {
        var records = await GetCategories().ConfigureAwait(false);

        var categories = records.Select(Category.FromEntity);

        return Result.Ok(new RetrieveCategoriesOutput(categories));
    }

    private async Task<List<CategoryEntity>> GetCategories()
    {
        return await _context
                    .Categories
                    .Include(x => x.SubCategories)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);
    }
}
