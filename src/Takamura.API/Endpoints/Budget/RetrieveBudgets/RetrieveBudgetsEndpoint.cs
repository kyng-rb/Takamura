using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.Retrieve;

namespace Takamura.API.Endpoints.Budget.RetrieveBudgets;

public static class RetrieveBudgetsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async ([FromServices] RetrieveBudgetsService service)
            => await service.Handle().ToHttp())
            .Produces<RetrieveBudgetsOutput>();
    }
}
