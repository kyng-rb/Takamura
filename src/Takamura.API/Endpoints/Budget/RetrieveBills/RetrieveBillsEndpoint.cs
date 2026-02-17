using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Bill.Retrieve;

namespace Takamura.API.Endpoints.Budget.RetrieveBills;

public static class RetrieveBillsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/bill", async ([FromRoute] int id, [AsParameters] RetrieveBillsRequest input, [FromServices] RetrieveBillsService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces<RetrieveBillsOutput>();
    }
}
