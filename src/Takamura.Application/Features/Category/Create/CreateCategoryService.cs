using FluentResults;
using Takamura.Application.Database;
using Takamura.Application.Database.Entities.Base.Enums;
using Takamura.Application.Database.Entities.Category;

namespace Takamura.Application.Features.Category.Create;

public class CreateCategoryService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;

    public async Task<Result<int>> Handle(CreateCategoryServiceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
            return Result.Fail("Entry description cannot be empty.");

        var category = input.ToEntity();

        _context.Categories.Add(category);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return Result.Ok(category.Id);
    }
}

public record CreateCategoryServiceInput(string Description)
{
    public CategoryEntity ToEntity()
        => new()
        {
            Id = 0,
            Description = Description,
            CreatedAt = DateTime.UtcNow,
            Status = Status.Created
        };
}