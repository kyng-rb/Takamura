using Takamura.API.Extensions;
using Takamura.Application.Features.Category.Retrieve;

namespace Takamura.API.Endpoints.Category.RetrieveCategories;

public static class RetrieveCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (RetrieveCategoriesService service, CancellationToken cancellationToken)
            => await service.Handle(cancellationToken).ToHttp())
            .Produces<RetrieveCategoriesOutput>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
