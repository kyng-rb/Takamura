using RestSharp;
using RestSharp.Extensions.DependencyInjection;
using Takamura.Web.Services.Infrastructure;
using Takamura.Web.Settings;

namespace Takamura.Web.Services.Category.Create;

public record CategoryCreateSubCategoryInput(string Description);

public record CategoryCreateInput(
    string Description,
    IReadOnlyCollection<CategoryCreateSubCategoryInput> SubCategories);

public class CategoryCreateService
{
    private readonly IRestClientFactory _restClientFactory;

    public CategoryCreateService(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
    }

    public async Task Handle(CategoryCreateInput input, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest("api/category", Method.Post);
        request.AddBody(new
        {
            input.Description,
            SubCategories = input.SubCategories.Select(x => new { x.Description }).ToArray()
        });

        var client = _restClientFactory.CreateClient(ApiSettings.ApiName);
        var response = await client.ExecutePostAsync(request, cancellationToken);
        RestResponseGuard.EnsureSuccess(response, request.Resource);
    }
}
