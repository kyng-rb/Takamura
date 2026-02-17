using Takamura.Application.Features.Category.AttachSubCategory;

namespace Takamura.API.Endpoints.Category.AttachSubCategory;

public record AttachSubCategoryRequest(string Description)
{
    public AttachSubCategoryServiceInput ToServiceInput(int categoryId) => new(categoryId, Description);
}
