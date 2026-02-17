using Takamura.API.Extensions;
using Takamura.Application.Features.Category.Retrieve;

namespace Takamura.API.Endpoints.Category.RetrieveCategories;

public static class RetrieveCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (RetrieveCategoriesService service)
            => await service.Handle().ToHttp())
            .Produces<RetrieveCategoriesOutput>();
    }
}
