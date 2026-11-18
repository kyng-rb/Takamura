using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.Retrieve;

namespace Takamura.API.Endpoints.Budget.RetrieveBudgets;

public static class RetrieveBudgetsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async ([FromServices] RetrieveBudgetsService service, CancellationToken cancellationToken)
            => await service.Handle(cancellationToken).ToHttp())
            .Produces<RetrieveBudgetsOutput>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
