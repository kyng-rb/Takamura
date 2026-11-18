using Takamura.Application.Features.Category.Create;

namespace Takamura.API.Endpoints.Category.CreateCategory;

public record CreateCategorySubCategoryRequest(string Description);

public record CreateCategoryRequest(string Description, CreateCategorySubCategoryRequest[]? SubCategories)
{
    public CreateCategoryServiceInput ToServiceInput()
        => new(Description, SubCategories?.Select(x => new CreateSubCategoryInput(x.Description)).ToArray() ?? []);
}
