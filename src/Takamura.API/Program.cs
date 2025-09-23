using Takamura.API;
using Takamura.Application;

var builder = WebApplication.CreateBuilder(args);

var databaseConnectionString = builder.Configuration.GetConnectionString("TakamuraConnectionString");

builder.Services.AddDatabaseContext(databaseConnectionString!);
builder.Services.AddServices();

var app = builder.Build();
app.UseHttpsRedirection();

var endpoints = new Endpoints();
endpoints.Map(app);

app.Run();