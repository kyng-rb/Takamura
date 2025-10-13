using FluentResults;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Features.Category.Create;

public class CreateCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(CreateCategoryServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Entry description cannot be empty.");

        if (input.SubCategories.Any(x => string.IsNullOrWhiteSpace(x.Description)))
            return Result.Fail("SubCategory description cannot be empty.");

        var category = input.ToEntity();

        _context.Categories.Add(category);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok(category.Id);
    }
}

public record CreateSubCategoryInput(string Description);

public record CreateCategoryServiceInput(string Description, IEnumerable<CreateSubCategoryInput> SubCategories)
{
    public CategoryEntity ToEntity()
    {
        var category = CategoryEntity.Create(Description);

        if (SubCategories?.Count() > 0)
        {
            category.AttachSubCategories(SubCategories.Select(x => x.Description));
        }

        return category;
    }
}