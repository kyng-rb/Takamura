using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Features.Category.Retrieve;

public record RetrieveCategoriesOutput(IEnumerable<CategoryOutput> Categories);

public record SubCategoryOutput(int Id, string Description);

public record CategoryOutput(int Id, string Description, IEnumerable<SubCategoryOutput> SubCategories)
{
    public static CategoryOutput FromEntity(CategoryEntity entity)
    {
        var subCategories = entity.SubCategories.Select(x => new SubCategoryOutput(x.Id, x.Description));
        return new CategoryOutput(entity.Id, entity.Description, subCategories);
    }
}

public class RetrieveCategoriesService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<RetrieveCategoriesOutput>> Handle(CancellationToken cancellationToken = default)
    {
        var records = await GetCategories(cancellationToken).ConfigureAwait(false);

        var categories = records.Select(CategoryOutput.FromEntity);

        return Result.Ok(new RetrieveCategoriesOutput(categories));
    }

    private async Task<List<CategoryEntity>> GetCategories(CancellationToken cancellationToken)
    {
        return await _context
                    .Categories
                    .Include(x => x.SubCategories)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
    }
}
