using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.Create;

namespace Takamura.API.Endpoints.Budget.CreateBudget;

public static class CreateBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async ([FromBody] CreateBudgetRequest input, [FromServices] CreateBudgetService service, CancellationToken cancellationToken)
            => await service.Handle(input.ToServiceInput(), cancellationToken).ToCreated())
            .Produces<CreateBudgetOutput>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
