namespace Takamura.API.Endpoints;

public static class EndpointsMapperExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "Takamura API is running...");

        CategoryEndpoints.Map(app);
        BudgetEndpoints.Map(app);
        return app;
    }
}