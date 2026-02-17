using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Bill.Create;

namespace Takamura.API.Endpoints.Budget.CreateBill;

public static class CreateBillEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id}/bill", async ([FromRoute] int id, [FromBody] CreateBillRequest input, [FromServices] CreateBillService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces(StatusCodes.Status204NoContent);
    }
}
