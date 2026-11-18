using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Bill.Create;

namespace Takamura.API.Endpoints.Budget.CreateBill;

public static class CreateBillEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/bills", async ([FromRoute] int id, [FromBody] CreateBillRequest input, [FromServices] CreateBillService service, CancellationToken cancellationToken)
            => await service.Handle(input.ToServiceInput(id), cancellationToken).ToCreated())
            .Produces<CreateBillOutput>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
