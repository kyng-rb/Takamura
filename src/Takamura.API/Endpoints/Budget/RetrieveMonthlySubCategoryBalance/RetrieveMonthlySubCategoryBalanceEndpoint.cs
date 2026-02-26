using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Budget.RetrieveMonthlySubCategoryBalance;

namespace Takamura.API.Endpoints.Budget.RetrieveMonthlySubCategoryBalance;

public static class RetrieveMonthlySubCategoryBalanceEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/allocation/subcategorybalance", async ([FromRoute] int id, [AsParameters] RetrieveMonthlySubCategoryBalanceRequest input, [FromServices] RetrieveMonthlySubCategoryBalanceService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces<RetrieveMonthlySubCategoryBalanceOutput>();
    }
}
