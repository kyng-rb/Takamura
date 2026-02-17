using Takamura.API.Extensions;
using Takamura.Application.Features.Category.Create;

namespace Takamura.API.Endpoints.Category.CreateCategory;

public static class CreateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateCategoryService service, CreateCategoryServiceInput input)
            => await service.Handle(input).ToHttp())
            .Produces(StatusCodes.Status204NoContent);
    }
}
