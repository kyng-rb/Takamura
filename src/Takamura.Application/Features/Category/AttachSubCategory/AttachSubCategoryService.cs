using FluentResults;
using Takamura.Application.Database;

namespace Takamura.Application.Features.Category.AttachSubCategory;

public record AttachSubCategoryServiceInput(int CategoryId, string Description);

public class AttachSubCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result> Handle(AttachSubCategoryServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Entry description cannot be empty.");

        var category = await _context.Categories.FindAsync(input.CategoryId).ConfigureAwait(false);
        if (category is null)
            return Result.Fail("Category not found.");

        category.AttachSubCategory(input.Description);
        _context.Categories.Update(category);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok();
    }
}
