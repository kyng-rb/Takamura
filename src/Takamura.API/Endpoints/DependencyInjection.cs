namespace Takamura.API.Endpoints;

using Budget;
using Category;

public static class EndpointsMapperExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "Takamura API is running...");

        CategoryEndpointGroup.Map(app);
        BudgetEndpointGroup.Map(app);
        return app;
    }
}
