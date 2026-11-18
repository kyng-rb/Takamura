using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Settings;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Services.Bills.Retrieve;

public record BillsRetrieveServiceInput(
    DateOnly? From = null,
    DateOnly? To = null,
    int? CategoryId = null,
    int? SubCategoryId = null
);

public record BillsRetrieveItem(
    int Id,
    string Description,
    decimal Amount,
    DateOnly Date,
    string Category,
    string SubCategory);

public record BillsRetrieveOutput(BillsRetrieveItem[] Bills);

public class BillsRetrieveService
{
    private readonly IRestClientFactory _restFactory;

    public BillsRetrieveService(IRestClientFactory restFactory)
    {
        _restFactory = restFactory;
    }

    public async Task<BillsRetrieveOutput> Handle(
        int budgetId,
        BillsRetrieveServiceInput? input = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"api/budgets/{budgetId}/bills");
        if (input is not null)
        {
            if (input.From is not null)
                request.AddQueryParameter("From", input.From.Value);

            if (input.To is not null)
                request.AddQueryParameter("To", input.To.Value);

            if (input.CategoryId is not null)
                request.AddQueryParameter("CategoryId", input.CategoryId.Value);

            if (input.SubCategoryId is not null)
                request.AddQueryParameter("SubCategoryId", input.SubCategoryId.Value);
        }

        var client = _restFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecuteGetAsync<BillsRetrieveOutput>(request, cancellationToken);

        return RestResponseGuard.EnsureSuccessAndData(response, request.Resource);
    }
}
