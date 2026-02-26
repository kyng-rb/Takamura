using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Settings;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Services.Category.Retrieve;

public record CategoryRetrieveOutput(CategoryItem[] Categories);

public record CategoryItem(int Id, string Description, SubCategoryRetrieveItem[]? SubCategories);

public record SubCategoryRetrieveItem(int Id, string Description);

public class CategoryRetrieveService
{
    private readonly IRestClientFactory _httpFactory;

    public CategoryRetrieveService(IRestClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<CategoryRetrieveOutput> Handle(CancellationToken cancellationToken = default)
    {
        var request = new RestRequest("api/category");
        var client = _httpFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecuteAsync<CategoryRetrieveOutput>(request, cancellationToken);
        return RestResponseGuard.EnsureSuccessAndData(response, request.Resource);
    }
}
