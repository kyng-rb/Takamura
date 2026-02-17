using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.Retrieve;

namespace Takamura.API.Endpoints.Budget.RetrievePeriodAllocations;

public static class RetrievePeriodAllocationsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id}/allocation", async ([FromRoute] int id, [AsParameters] RetrievePeriodAllocationRequest input, [FromServices] RetrievePeriodAllocationsService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces<RetrievePeriodAllocationsOutput>();
    }
}
