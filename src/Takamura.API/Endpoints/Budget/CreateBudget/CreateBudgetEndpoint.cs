using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.Create;

namespace Takamura.API.Endpoints.Budget.CreateBudget;

public static class CreateBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async ([FromBody] CreateBudgetServiceInput input, [FromServices] CreateBudgetService service)
            => await service.Handle(input).ToHttp())
            .Produces(StatusCodes.Status204NoContent);
    }
}
