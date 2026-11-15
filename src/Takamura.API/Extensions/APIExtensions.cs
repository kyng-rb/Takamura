using Scalar.AspNetCore;
using Takamura.API.Endpoints;
using Takamura.Application;

namespace Takamura.API.Extensions;

public static class ApiExtensions
{
    public static WebApplication ConfigureApi(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Unhandled exceptions -> 500 problem details (instead of an empty body).
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseHttpsRedirection();

        app.MapEndpoints();

        app.MapOpenApi();

        app.MapScalarApiReference(options => options
            .WithTitle("Takamura API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithLayout(ScalarLayout.Modern)
            .WithSidebar(false)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp));
        return app;
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        // Model-binding failures (bad JSON, wrong types) and errors return RFC 7807 problem details.
        builder.Services.AddProblemDetails();

        var databaseConnectionString = builder.Configuration.GetConnectionString("TakamuraConnectionString");
        var isDevelopment = builder.Environment.IsDevelopment();

        builder.Services.AddDatabaseContext(databaseConnectionString!, isDevelopment);
        builder.Services.AddApplicationServices();
        return builder;
    }
}
