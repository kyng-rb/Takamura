using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.RetrievePeriodAllocations;

namespace Takamura.API.Endpoints.Budget.RetrievePeriodAllocations;

public static class RetrievePeriodAllocationsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/allocations", async ([FromRoute] int id, [AsParameters] RetrievePeriodAllocationRequest input, [FromServices] RetrievePeriodAllocationsService service, CancellationToken cancellationToken)
            => await service.Handle(input.ToServiceInput(id), cancellationToken).ToHttp())
            .Produces<RetrievePeriodAllocationsOutput>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
