using Takamura.Application;
using Takamura.Application.Features.Category.Retrieve;

var builder = WebApplication.CreateBuilder(args);

var databaseConnectionString = builder.Configuration.GetConnectionString("TakamuraConnectionString");

builder.Services.AddDatabaseContext(databaseConnectionString!);
builder.Services.AddServices();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "App is running");

app.MapGet("api/category", async (RetrieveCategoriesService service) => await service.Handle());

app.Run();