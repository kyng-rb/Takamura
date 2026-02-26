using RestSharp;
using RestSharp.Extensions.DependencyInjection;

namespace Takamura.Web.Settings;

public static class SettingsExtensions
{
    public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
    {
        var api = configuration.GetRequiredSection(ApiSettings.FieldName).Get<ApiSettings>()!;

        var options = new RestClientOptions
        {
            BaseUrl = new Uri(api.Host),
            Timeout = TimeSpan.FromSeconds(120)
        };

        services.AddRestClient(ApiSettings.ApiName, options);
        return services;
    }
}