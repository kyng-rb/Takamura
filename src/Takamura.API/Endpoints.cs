using Takamura.API.Extensions;
using Takamura.Application.Features.Category.Create;
using Takamura.Application.Features.Category.Retrieve;

namespace Takamura.API;

public class Endpoints
{
    public const string Base = "api";

    public void Map(WebApplication app)
    {
        app.MapGet("/", () => "App is running");

        app.MapGet($"{Base}/category", async (RetrieveCategoriesService service)
            => await service.Handle().ToHttp());

        app.MapPost($"{Base}/category", async (CreateCategoryService service, CreateCategoryServiceInput input)
            => await service.Handle(input).ToHttp());
    }
}