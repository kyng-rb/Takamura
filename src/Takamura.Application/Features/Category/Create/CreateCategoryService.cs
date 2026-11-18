using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Features.Category.Create;

public record CreateSubCategoryInput(string Description);

public record CreateCategoryServiceInput(string Description, IEnumerable<CreateSubCategoryInput> SubCategories)
{
    public CategoryEntity ToEntity()
    {
        var category = CategoryEntity.Create(Description);

        if (SubCategories?.Any() == true)
        {
            category.AttachSubCategories(SubCategories.Select(x => x.Description));
        }

        return category;
    }
}

public record CreateCategoryOutput(int Id);

public class CreateCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<CreateCategoryOutput>> Handle(CreateCategoryServiceInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Description is required");

        if (input.Description.Length > FieldLimits.TextMaxLength)
            return Result.Fail($"Description must be at most {FieldLimits.TextMaxLength} characters");

        if (input.SubCategories?.Any(x => string.IsNullOrWhiteSpace(x.Description)) == true)
            return Result.Fail("SubCategory description is required");

        if (input.SubCategories?.Any(x => x.Description.Length > FieldLimits.TextMaxLength) == true)
            return Result.Fail($"SubCategory description must be at most {FieldLimits.TextMaxLength} characters");

        var subCategoryNames = input.SubCategories?.Select(x => x.Description.Trim()).ToArray() ?? [];
        if (subCategoryNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != subCategoryNames.Length)
            return Result.Fail("SubCategory descriptions must be unique");

        if (await _context.Categories.AnyAsync(x => x.Description == input.Description, cancellationToken).ConfigureAwait(false))
            return Result.Fail("A category with this description already exists");

        var category = input.ToEntity();

        _context.Categories.Add(category);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Ok(new CreateCategoryOutput(category.Id));
    }
}
