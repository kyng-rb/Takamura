using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;

namespace Takamura.Application.Features.Category.AttachSubCategory;

public record AttachSubCategoryServiceInput(int CategoryId, string Description);

public record AttachSubCategoryOutput(int Id);

public class AttachSubCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<AttachSubCategoryOutput>> Handle(AttachSubCategoryServiceInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Description is required");

        if (input.Description.Length > FieldLimits.TextMaxLength)
            return Result.Fail($"Description must be at most {FieldLimits.TextMaxLength} characters");

        var category = await _context.Categories.FindAsync([input.CategoryId], cancellationToken).ConfigureAwait(false);
        if (category is null)
            return Result.Fail("Category does not exist");

        if (await _context.SubCategories.AnyAsync(x => x.CategoryId == input.CategoryId && x.Description == input.Description, cancellationToken).ConfigureAwait(false))
            return Result.Fail("This category already has a subcategory with this description");

        var subCategory = category.AttachSubCategory(input.Description);
        // The category is tracked (FindAsync), so only the new subcategory is inserted.
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Ok(new AttachSubCategoryOutput(subCategory.Id));
    }
}
