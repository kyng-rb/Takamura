using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Services.Infrastructure;
using Takamura.Web.Settings;

namespace Takamura.Web.Services.Category.AddSubCategory;

public record CategoryAddSubCategoryInput(int CategoryId, string Description);

public class CategoryAddSubCategoryService
{
    private readonly IRestClientFactory _restClientFactory;

    public CategoryAddSubCategoryService(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
    }

    public async Task Handle(CategoryAddSubCategoryInput input, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"api/category/{input.CategoryId}", Method.Post);
        request.AddBody(new
        {
            input.Description
        });

        var client = _restClientFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecutePostAsync(request, cancellationToken);
        RestResponseGuard.EnsureSuccess(response, request.Resource);
    }
}
