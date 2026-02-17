using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.AttachPeriod;

namespace Takamura.API.Endpoints.Budget.AttachPeriodAllocation;

public static class AttachPeriodAllocationEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id}/allocation", async ([FromRoute] int id, [FromBody] AttachPeriodAllocationRequest input, [FromServices] AttachPeriodAllocationService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces(StatusCodes.Status204NoContent);
    }
}
