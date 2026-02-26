using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Settings;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Services.Allocation.Create;

public record AllocationCreateInput(
    int BudgetId,
    int SubCategoryId,
    int Month,
    int Year,
    string Description,
    decimal Amount,
    string Type);

public class AllocationCreateService
{
    private readonly IRestClientFactory _restClientFactory;

    public AllocationCreateService(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
    }

    public async Task Handle(AllocationCreateInput input, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"api/budget/{input.BudgetId}/allocation", Method.Post);
        request.AddBody(new
        {
            input.SubCategoryId,
            input.Month,
            input.Year,
            input.Description,
            input.Amount,
            input.Type
        });

        var client = _restClientFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecutePostAsync(request, cancellationToken);

        RestResponseGuard.EnsureSuccess(response, request.Resource);
    }
}
