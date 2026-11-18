using FluentResults;
using Takamura.Application.Database;

namespace Takamura.Application.Features.Category.AttachSubCategory;

public record AttachSubCategoryServiceInput(int CategoryId, string Description);

public record AttachSubCategoryOutput(int Id);

public class AttachSubCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<AttachSubCategoryOutput>> Handle(AttachSubCategoryServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Entry description cannot be empty.");

        if (input.Description.Length > FieldLimits.TextMaxLength)
            return Result.Fail($"Description must be at most {FieldLimits.TextMaxLength} characters.");

        var category = await _context.Categories.FindAsync(input.CategoryId).ConfigureAwait(false);
        if (category is null)
            return Result.Fail("Category not found.");

        var subCategory = category.AttachSubCategory(input.Description);
        _context.Categories.Update(category);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok(new AttachSubCategoryOutput(subCategory.Id));
    }
}
