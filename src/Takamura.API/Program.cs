using Takamura.API.Endpoints;
using Takamura.Application;

var builder = WebApplication.CreateBuilder(args);

var databaseConnectionString = builder.Configuration.GetConnectionString("TakamuraConnectionString");

builder.Services.AddDatabaseContext(databaseConnectionString!);
builder.Services.AddServices();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/", () => "Takamura API is running...");

CategoryEndpoints.Map(app);
BudgetEndpoints.Map(app);

app.Run();