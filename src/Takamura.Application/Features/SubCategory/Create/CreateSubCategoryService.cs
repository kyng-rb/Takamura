using FluentResults;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Base.Enums;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Features.SubCategory.Create;

public class CreateSubCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(CreateSubCategoryServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Entry description is required");
        
        if (input.CategoryId <= 0)
            return Result.Fail("Category id is required");
        
        if (!_context.Categories.Any(category => category.Id == input.CategoryId))
            return Result.Fail("Category does not exist");
        
        var entity = input.ToEntity();
        
        _context.SubCategories.Add(entity);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok(entity.Id);
    }
}

public record CreateSubCategoryServiceInput(int CategoryId, string Description)
{
    public SubCategoryEntity ToEntity()
    {
        return new SubCategoryEntity
        {
            Id = 0,
            Description = Description,
            CategoryId = CategoryId,
            CreatedAt = DateTime.UtcNow,
            Status = Status.Created
        };
    }
}