using Scalar.AspNetCore;
using Takamura.API.Endpoints;
using Takamura.Application;

namespace Takamura.API.Extensions;

public static class APIExtensions
{
    public static WebApplication ConfigureAPI(this WebApplicationBuilder builder)
    {
        var app = builder.Build();
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

        var databaseConnectionString = builder.Configuration.GetConnectionString("TakamuraConnectionString");

        builder.Services.AddDatabaseContext(databaseConnectionString!);
        builder.Services.AddApplicationServices();
        return builder;
    }
}