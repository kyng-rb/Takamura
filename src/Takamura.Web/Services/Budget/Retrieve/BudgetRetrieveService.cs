using System.Text.Json.Serialization;
using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Settings;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Services.Budget.Retrieve;

public record BudgetRetrieveOutput(BudgetRetrieveItem[]? Budgets);

public class BudgetRetrieveItem
{
    public int Id { get; set; }
    
    public required string Title { get; set; }
}

public class BudgetRetrieveService
{
    private readonly IRestClientFactory _httpFactory;

    public BudgetRetrieveService(IRestClientFactory factory)
    {
        _httpFactory = factory;
    }

    public async Task<BudgetRetrieveOutput> Handle(CancellationToken cancellationToken = default)
    {
        var request = new RestRequest("api/budget");
        var client = _httpFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecuteAsync<BudgetRetrieveOutput>(request, cancellationToken);

        return RestResponseGuard.EnsureSuccessAndData(response, request.Resource);
    }
}
