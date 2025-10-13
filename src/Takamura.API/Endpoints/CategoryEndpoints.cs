using Takamura.API.Extensions;
using Takamura.Application.Features.Category.AttachSubCategory;
using Takamura.Application.Features.Category.Create;
using Takamura.Application.Features.Category.Retrieve;

namespace Takamura.API.Endpoints;

public static class CategoryEndpoints
{
    private const string Section = "api/category";

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup(Section);

        group.MapGet("/", async (RetrieveCategoriesService service)
            => await service.Handle().ToHttp());

        group.MapPost("/", async (CreateCategoryService service, CreateCategoryServiceInput input)
            => await service.Handle(input).ToHttp());

        group.MapPost($"/{{id}}", async (AttachSubCategoryService service, int id, AttachSubCategoryRequest input)
            => await service.Handle(input.ToServiceInput(id)).ToHttp());
    }
}

public record AttachSubCategoryRequest(string Description)
{
    public AttachSubCategoryServiceInput ToServiceInput(int categoryId) => new(categoryId, Description);
};