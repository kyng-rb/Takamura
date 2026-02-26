using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Settings;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Services.Allocation.Retrieve;

public record AllocationRetrieveItem(
    int Id,
    decimal Amount,
    int Month,
    int Year,
    string Description,
    string Type,
    int SubCategoryId,
    string SubCategory,
    int CategoryId,
    string Category);

public record AllocationRetrieveOutput(AllocationRetrieveItem[] Allocations);

public class AllocationRetrieveService
{
    private readonly IRestClientFactory _restClientFactory;

    public AllocationRetrieveService(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
    }

    public async Task<AllocationRetrieveOutput> Handle(
        int budgetId,
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"api/budget/{budgetId}/allocation");
        if (year is not null)
            request.AddQueryParameter("Year", year.Value);

        var client = _restClientFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecuteGetAsync<AllocationRetrieveOutput>(request, cancellationToken);

        return RestResponseGuard.EnsureSuccessAndData(response, request.Resource);
    }
}
