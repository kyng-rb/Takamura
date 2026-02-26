using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Services.Infrastructure;
using Takamura.Web.Settings;

namespace Takamura.Web.Services.Balances.RetrieveSubCategoryBalance;

public record RetrieveSubCategoryBalanceItem(
    string Budget,
    decimal PeriodBudget,
    decimal AvailableAmount,
    int Month,
    int Year,
    string Category,
    string SubCategory);

public record RetrieveSubCategoryBalanceOutput(
    RetrieveSubCategoryBalanceItem[] Balances);

public class RetrieveSubCategoryBalanceService
{
    private readonly IRestClientFactory _restClientFactory;

    public RetrieveSubCategoryBalanceService(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
    }

    public async Task<RetrieveSubCategoryBalanceOutput> Handle(
        int budgetId,
        int? year = null,
        int? month = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"api/budget/{budgetId}/allocation/subcategorybalance");
        if (year is not null)
            request.AddQueryParameter("Year", year.Value);
        if (month is not null)
            request.AddQueryParameter("Month", month.Value);

        var client = _restClientFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecuteGetAsync<RetrieveSubCategoryBalanceOutput>(request, cancellationToken);

        return RestResponseGuard.EnsureSuccessAndData(response, request.Resource);
    }
}
