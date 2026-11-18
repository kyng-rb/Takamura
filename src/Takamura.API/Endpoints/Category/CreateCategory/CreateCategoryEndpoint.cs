using Takamura.API.Extensions;
using Takamura.Application.Features.Category.Create;

namespace Takamura.API.Endpoints.Category.CreateCategory;

public static class CreateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateCategoryService service, CreateCategoryServiceInput input, CancellationToken cancellationToken)
            => await service.Handle(input, cancellationToken).ToCreated())
            .Produces<CreateCategoryOutput>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
