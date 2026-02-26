using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Settings;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Services.Bills.Create;

public record BillsCreateServiceInput(
    int BudgetId,
    DateOnly Date,
    string Description,
    decimal Amount,
    int SubCategoryId);

public class BillsCreateService
{
    private readonly IRestClientFactory _restClientFactory;

    public BillsCreateService(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
    }

    public async Task Handle(BillsCreateServiceInput input, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"api/budget/{input.BudgetId}/bill", Method.Post);
        request.AddBody(new
        {
            input.Date,
            input.Description,
            input.Amount,
            input.SubCategoryId
        });

        var client = _restClientFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecutePostAsync(request, cancellationToken);
        RestResponseGuard.EnsureSuccess(response, request.Resource);
    }
}
